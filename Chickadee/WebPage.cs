using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tree_matching_csharp;
using System;


namespace Chickadee
{
    public class WebPage
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public bool isVisited { get; set; }
        public bool didRequestSucceed { get; set; }
        public bool isUaComputed { get; set; }
        public virtual ICollection<WebPageVisit> WebPageVisits { get; set; }

        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static WebPage GetWebPage()
        {
            WebPage webPage = null;
            using (var context = new LibraryContext())
            {
                webPage = context.WebPage.Where(w => w.isUaComputed.Equals(false)).Where(w => w.didRequestSucceed.Equals(true)).OrderBy(w => w.ID).FirstOrDefault();
            }
            return webPage;
        }

        public static void UpdateWebPage(WebPage page)
        {
            using (var dbContext = new LibraryContext())
            {
                try
                {
                    dbContext.Update(page);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error message: {e.Message}");
                    Console.WriteLine($"inner exception: {e.InnerException}");
                }
            }
        }
        public static async Task<IEnumerable<Node>> GetDomTree(string filePath)
        {
            var webPageDocument = await WebSurfer.GetWebPageDocument(filePath);
            var domTree = WebSurfer.GetWebPageDomTree(webPageDocument);

            return domTree;
        }
    }
}
