using System;
using System.Collections.Generic;
using WCFService;
using WCFService.DataLayer;

namespace WCFServiceTests1.Stubs
{
    public class StubTemperatureRepository : ITemperatureRepository
    {
        public List<Temperature> GetTemperatures(string start, string end)
        {
            return null;

        }

        public void InsertTemperature(int temperature, string sensorID)
        {
            MostRecentTemperature = new Tuple<int, string>(temperature, sensorID);
        }

        public Tuple<int, string> MostRecentTemperature { get; private set; }
    }
}