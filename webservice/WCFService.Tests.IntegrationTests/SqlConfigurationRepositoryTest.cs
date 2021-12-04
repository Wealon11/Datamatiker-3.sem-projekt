using System;
using WCFService.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RimDev.Automation.Sql;

namespace WCFService.Tests.IntegrationTests
{
    [TestClass]
    public class SqlConfigurationRepositoryTest
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
        public void TestGetNonexistentConfKeyShouldReturnNull()
        {
            var configRepo = new SqlConfigurationRepository(_db.ConnectionString);
            var confValue = configRepo.Get("non-existent-key");
            Assert.IsNull(confValue);
        }

        [TestMethod]
        public void TestGetExistingConf()
        {
            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"insert into Configuration (ConfKey, ConfValue) values ('x', 'y')";
                    var rowsAffected = cmd.ExecuteNonQuery();
                    Assert.AreEqual(1, rowsAffected);
                }
            }
            var configRepo = new SqlConfigurationRepository(_db.ConnectionString);
            var confValue = configRepo.Get("x");
            Assert.AreEqual("y", confValue);
        }

        [TestMethod]
        public void TestSetNew()
        {
            var configRepo = new SqlConfigurationRepository(_db.ConnectionString);

            var confKey = "foo";
            var expectedConfValue = "bar";

            configRepo.Set(confKey, expectedConfValue);

            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select ConfValue from Configuration where ConfKey = '{confKey}'";
                    var r = cmd.ExecuteReader();
                    Assert.IsTrue(r.Read());
                    var actualConfValue = r.GetString(0);
                    Assert.AreEqual(expectedConfValue, actualConfValue);
                }
            }
        }

        [TestMethod]
        public void TestSetExisting()
        {
            var confKey = "foo";
            var expectedConfValue = "new value";

            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"insert into Configuration (ConfKey, ConfValue) values ('{confKey}', 'old value')";
                    var rowsAffected = cmd.ExecuteNonQuery();
                    Assert.AreEqual(1, rowsAffected);
                }
            }

            var configRepo = new SqlConfigurationRepository(_db.ConnectionString);

            configRepo.Set(confKey, expectedConfValue);

            using (var conn = _db.OpenConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"select ConfValue from Configuration where ConfKey = '{confKey}'";
                    var r = cmd.ExecuteReader();
                    Assert.IsTrue(r.Read());
                    var actualConfValue = r.GetString(0);
                    Assert.AreEqual(expectedConfValue, actualConfValue);
                }
            }
        }

        [TestMethod]
        public void TestSetNullArgumentShouldThrow()
        {
            var configRepo = new SqlConfigurationRepository(_db.ConnectionString);

            Assert.ThrowsException<ArgumentException>(() => configRepo.Set(null, null));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set("ok", null));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set(null, "ok"));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set("", null));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set(null, ""));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set("", ""));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set(" ", "ok"));
            Assert.ThrowsException<ArgumentException>(() => configRepo.Set("ok", " "));
        }
    }
}
