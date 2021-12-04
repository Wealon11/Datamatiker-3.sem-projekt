using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFService
{
    public class Temperature
    {
        [DataMember] public int temps;
        [DataMember] public int ID;
        [DataMember] public DateTime Date;
        [DataMember] public string SensorID;
    }
}