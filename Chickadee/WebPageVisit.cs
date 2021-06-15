using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Threading.Tasks;

namespace Chickadee
{
    public class WebPageVisit
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public virtual string ConfigurationUid { get; set; }
        public DateTime VisitTime { get; set; }

        public bool IsDomProcessed { get; set; }
        public string FilePath { get; set; }
        public virtual WebPage WebPage { get; set; }
        public virtual ICollection<DomChange> ua1DomChanges { get; set; }
        public virtual ICollection<DomChange> ua2DomChanges { get; set; }

        public static Logger logger = LogManager.GetCurrentClassLogger();

        public static void UpdateWebPageVisit(WebPageVisit visit, WebPage page)
        {
            var wv = new WebPageVisit()
            {
                ID = visit.ID,
                Url = visit.Url,
                ConfigurationUid = visit.ConfigurationUid,
                VisitTime = visit.VisitTime,
                IsDomProcessed = visit.IsDomProcessed,
                FilePath = visit.FilePath,
                WebPage = page
            };
            using (var dbContext = new LibraryContext())
            {
                try
                {
                    dbContext.Update(wv);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error message: {e.Message}");
                    Console.WriteLine($"inner exception: {e.InnerException}");
                }
            }
        }

        public static WebPageVisit GetVisitByUa(string url, string ua)
        {
            WebPageVisit webPageVisit = null;
            using (var context = new LibraryContext())
            {
                try
                {
                    webPageVisit = context.WebPageVisit.Where(v => v.Url.Equals(url)).Where(v => v.ConfigurationUid.Equals(ua)).Where(v => v.IsDomProcessed.Equals(false)).OrderBy(v => v.VisitTime).FirstOrDefault();
                }
                catch (Exception e)
                {
                    logger.Error($"Couldn't find a visit at {url} {ua}: { e.Message }");
                    Console.WriteLine($"Couldn't find a visit at {url} {ua}: {e.Message}");
                    Console.WriteLine($"Error inner exception: { e.InnerException }");
                }
            }

            return webPageVisit;
        }
    }
}
