using System.Collections.Generic;
using System.Linq;

namespace Chickadee
{
    public class Domain
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public static List<Domain> GetUrls()
        {
            List<Domain> urls;
            using (var context = new LibraryContext())
            {
                urls = context.Domain.AsEnumerable<Domain>().Where(d => d.ID.Equals(427404)).OrderBy(domain => domain.ID).Take(1).ToList();
            }
            return urls;
        }
    }
}
