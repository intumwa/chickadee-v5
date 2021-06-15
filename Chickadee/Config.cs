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

        public static List<Config> GetConfigs()
        {
            List<Config> configs;
            using (var context = new LibraryContext())
            {
                configs = context.Config.AsEnumerable<Config>().OrderBy(config => config.ID).ToList();
            }
            return configs;
        }

        public static List<string> GetComparisons()
        {
            var configs = GetConfigs();
            List<string> pool = new List<string>();
            foreach (var ua1 in configs)
            {
                foreach (var ua2 in configs)
                {
                    var candidate = $"{ua1.UID}--{ua2.UID}";
                    var flip = $"{ua2.UID}--{ua1.UID}";
                    if (!ua1.UID.Equals(ua2.UID) && !pool.Contains(candidate) && !pool.Contains(flip)) pool.Add(candidate);
                }
            }
            return pool;
        }
    }
}
