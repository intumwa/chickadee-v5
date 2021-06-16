using System.Collections.Generic;
using System.Linq;

namespace Chickadee
{
    public class Config
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string ConfigType { get; set; }
        public string ConfigValue { get; set; }

        public static Config GetGeneric()
        {
            Config config;
            using (var db = new LibraryContext())
            {
                config = db.Config.AsEnumerable<Config>().Where(c => c.UID.Equals("Generic")).FirstOrDefault();
            }
            return config;
        }

        public static List<Config> GetConfigs(Config generic)
        {
            List<Config> configs;
            using (var context = new LibraryContext())
            {
                configs = context.Config.AsEnumerable<Config>().Where(c => !c.UID.Equals(generic.UID)).OrderBy(config => config.ID).ToList();
            }
            return configs;
        }
    }
}
