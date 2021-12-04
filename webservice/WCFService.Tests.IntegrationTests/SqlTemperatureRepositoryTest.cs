using System;
using WCFService.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RimDev.Automation.Sql;

namespace WCFService.Tests.IntegrationTests
{
    [TestClass]
    public class SqlTemperatureRepositoryTest
    {
        private LocalDb _db;

        [TestInitialize]
        public void TestInitialize()
        {
            _db = new LocalDb(version: "v13.0");
            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE [dbo].[UserInfo] (
    [Username] NVARCHAR (50)  NOT NULL,
    [Password] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Username] ASC)
);

CREATE TABLE [dbo].[TempDB] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [Dato]       DATETIME NOT NULL,
    [Temperatur] INT      NOT NULL,
    [Sensor ID]  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Configuration] (
    [ConfKey]   NVARCHAR (50)  NOT NULL,
    [ConfValue] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([ConfKey] ASC)
);

CREATE TABLE [dbo].[MailDB] (
    [Mail] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Mail] ASC)
);";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _db.Dispose();
            _db = null;
        }

        [TestMethod]
        public void TestInsertTemperature()
        {
            var insertedTemperature = 12345;
            var temperatureRepo = new SqlTemperatureRepository(_db.ConnectionString);
            temperatureRepo.InsertTemperature(insertedTemperature, "a");

            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select Id, Temperatur from TempDB";
                    var r = cmd.ExecuteReader();
                    Assert.IsTrue(r.Read());
                    var id = r.GetInt32(0);
                    var temperature = r.GetInt32(1);

                    Assert.AreEqual(1, id);
                    Assert.AreEqual(insertedTemperature, temperature);

                    Assert.IsFalse(r.Read()); // there should only be one row in result
                }
            }
        }

        [TestMethod]
        public void TestGetTemperatures()
        {
            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "insert into TempDB (Dato, Temperatur, [Sensor ID]) values ('1999-01-02', 5, 'a'), ('2100-01-01', 15, 'b'), ('2018-05-17', 10, 'c')";
                    var rowsAffected = cmd.ExecuteNonQuery();
                    Assert.AreEqual(3, rowsAffected);
                }
            }

            var temperatureRepo = new SqlTemperatureRepository(_db.ConnectionString);
            var temperaturesInRange = temperatureRepo.GetTemperatures("2018-01-01", "2018-12-31");

            Assert.AreEqual(1, temperaturesInRange.Count);
            var t = temperaturesInRange[0];
            Assert.AreEqual(new DateTime(2018, 5, 17), t.Date.Date);
            Assert.AreEqual(10, t.temps);
        }
    }
}
