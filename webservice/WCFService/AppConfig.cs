using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WCFService
{
    class AppConfig : IAppConfig
    {
        private AppConfig() { }

        public static AppConfig Instance = new AppConfig();

        public string ConnectionString => ConfigurationManager.ConnectionStrings["TempDB"].ConnectionString;
    }
}