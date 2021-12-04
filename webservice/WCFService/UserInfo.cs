using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFService
{
    public class UserInfo
    {
        [DataMember] public string User;
        [DataMember] public string Password;



    }
}