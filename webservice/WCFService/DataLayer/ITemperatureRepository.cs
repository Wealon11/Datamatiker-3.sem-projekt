using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFService.DataLayer
{
    public interface ITemperatureRepository
    {
        void InsertTemperature(int temperature, string sensorID);

        List<Temperature> GetTemperatures(string start, string end);

    }
}
