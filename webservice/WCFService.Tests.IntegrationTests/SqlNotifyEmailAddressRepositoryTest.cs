using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RimDev.Automation.Sql;
using WCFService.DataLayer;

namespace WCFService.Tests.IntegrationTests
{
    [TestClass]
    public class SqlNotifyEmailAddressRepositoryTest
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
        public void TestGetAllNoAddressesInTable()
        {
            var addrRepo = new SqlNotifyEmailAddressRepository(_db.ConnectionString);
            var addresses = addrRepo.GetAll();
            Assert.AreEqual(0, addresses.Count);
        }

        [TestMethod]
        public void TestGetAll()
        {
            using (var conn = new SqlConnection(_db.ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO MailDB (Mail) VALUES ('test@example.com')";
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            var addrRepo = new SqlNotifyEmailAddressRepository(_db.ConnectionString);

            var addresses = addrRepo.GetAll();
            Assert.AreEqual(1, addresses.Count);
            Assert.AreEqual("test@example.com", addresses[0]);
        }
    }
}
