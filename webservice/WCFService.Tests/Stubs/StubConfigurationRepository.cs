using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFService.DataLayer;

namespace WCFServiceTests1.Stubs
{
    class StubConfigurationRepository : IConfigurationRepository
    {
        public Dictionary<string, string> Config = new Dictionary<string, string>();

        public string Get(string key)
        {
            Config.TryGetValue(key, out string value);
            return value;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(key)} and {nameof(value)} parameters must be non-null and non-empty");
            }
            Config[key] = value;
        }
    }
}
