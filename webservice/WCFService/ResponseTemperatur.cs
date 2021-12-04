using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFService
{
    public class ResponseTemperatur
    {
        [DataMember] public string SensorID;
        [DataMember] public int temps;
        [DataMember] public int ID;
        [DataMember] public String Date;

        public ResponseTemperatur(Temperature temps)
        {
            this.SensorID = temps.SensorID;
            this.temps = temps.temps;
            this.ID = temps.ID;
            this.Date = temps.Date.ToString("o");
        }


        public ResponseTemperatur()
        {

        }

    }
}