using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFService.DataLayer;
using WCFService.Models;
using WCFServiceTests1.Stubs;
using WCFService.Services;

namespace WCFServiceTests1
{
    [TestClass()]
    public class Service1Tests
    {
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void GetMaxTempExeptionTest()
        {
            var configRepo = new StubConfigurationRepository();
            configRepo.Set("MaxTemp", "Karl-Marx");
            var service = new Service1(null, configRepo, new StubTemperatureRepository());
            service.GetMaxTemp();
        }

        [TestMethod]
        public void TestGetMaxTempDefaultValueIsZero()
        {
            var configRepo = new StubConfigurationRepository();
            var service = new Service1(null, configRepo, new StubTemperatureRepository());
            Assert.AreEqual(0, service.GetMaxTemp());
        }

        [TestMethod]
        public void TestGetMaxTemp()
        {
            var configRepo = new StubConfigurationRepository();
            configRepo.Set("MaxTemp", "10");

            var service = new Service1(null, configRepo, new StubTemperatureRepository());
            Assert.AreEqual(10, service.GetMaxTemp());
        }

        [TestMethod]
        public void TestUpdateMaxTemp()
        {
            var configRepo = new StubConfigurationRepository();
            var service = new Service1(null, configRepo, new StubTemperatureRepository());

            Assert.IsNull(configRepo.Get("MaxTemp"));
            service.UpdateMaxTemp(new MaxTemperatur { Maxtemps = "10" });
            Assert.AreEqual("10", configRepo.Get("MaxTemp"));
        }

        [TestMethod]
        public void TestUpdateMaxTempWithNonIntValueShouldFail()
        {
            var configRepo = new StubConfigurationRepository();
            var tempRepo = new StubTemperatureRepository();
            var service = new Service1(null, configRepo, tempRepo);

            Assert.IsNull(configRepo.Get("MaxTemp"));
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateMaxTemp(new MaxTemperatur { Maxtemps = "10.5" });
            });
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateMaxTemp(new MaxTemperatur { Maxtemps = "a" });
            });
        }

        [TestMethod]
        public void TestUpdateMaxTempWithNullArgumentShouldFail()
        {
            var configRepo = new StubConfigurationRepository();
            var service = new Service1(null, configRepo, new StubTemperatureRepository());

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                service.UpdateMaxTemp(null);
            });
        }

        [TestMethod]
        public void TestInsertTemperature()
        {
            var configRepo = new StubConfigurationRepository();
            var tempRepo = new StubTemperatureRepository();
            var discordBot = new StubDiscord();
            var service = new Service1(null, configRepo, tempRepo, new List<INotificationSender> { discordBot });

            var temperature = new IncomingTemperature { Temperature = 5, SensorID = "a" };
            service.InsertTemperature(temperature).Wait();

            Assert.AreEqual(temperature.Temperature, tempRepo.MostRecentTemperature.Item1);
            Assert.AreEqual(temperature.SensorID, tempRepo.MostRecentTemperature.Item2);
            Assert.AreEqual("5", discordBot.MostRecentMessage);
        }
    }
}