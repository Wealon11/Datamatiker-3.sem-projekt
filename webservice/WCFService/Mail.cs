using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFService
{
    public class Mail
    {
        [DataMember] public string mail;
    }
}