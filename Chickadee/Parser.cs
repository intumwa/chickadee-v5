using Chickadee;
using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;

namespace Chickadee
{
    public class Parser
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public async Task ComputeDomChanges(WebPageVisit ua1, WebPageVisit ua2, WebPage page)
        {
            Console.WriteLine($"processing DOM change for: { ua1.Url } {ua1.ConfigurationUid} vs. {ua2.ConfigurationUid}");
            try
            {
                var ua1File = FileHelper.BuildFilePath(ua1);
                var ua2File = FileHelper.BuildFilePath(ua2);

                if (File.Exists(ua1File) && File.Exists(ua2File))
                {
                    // check node changes and save them
                    var ua1DomTreeTask = WebPage.GetDomTree(ua1File);
                    var ua2DomTreeTask = WebPage.GetDomTree(ua2File);

                    var ua1DomTree = await ua1DomTreeTask;
                    var ua2DomTree = await ua2DomTreeTask;

                    var treeMatchResultTask = WebPageMatcher.MatchDomTrees(ua1DomTree, ua2DomTree);
                    var treeMatchResult = await treeMatchResultTask;
                    var treeEdges = treeMatchResult.Edges;

                    var domChange = DomChange.ProcessDomChange(treeEdges, ua1, ua2);
                    DomChange.SaveDomChange(domChange, ua1, ua2);

                    Console.WriteLine();
                } 
                else 
                {
                    Console.WriteLine($"either file {ua1File} or {ua2File} does not exist.");
                }

                //Update ua1 and ua2 web page visits, set IsFileProcessed and WebPageID
                ua1.IsDomProcessed = true;
                WebPageVisit.UpdateWebPageVisit(ua1, page);

                ua2.IsDomProcessed = true;
                WebPageVisit.UpdateWebPageVisit(ua2, page);

                page.isUaComputed = true;
                WebPage.UpdateWebPage(page);

                Console.WriteLine();
            }
            catch (Exception e)
            {
                logger.Error($"a problem occurred: { e.Message }");
                Console.WriteLine($"Error message: { e.Message }");
                Console.WriteLine($"Error inner exception: { e.InnerException }");
            }
        }

        public async Task ComputeUaChanges()
        {
            var webPage = WebPage.GetWebPage();
            var url = webPage.Url;
            Console.WriteLine($"url id: {webPage.ID}, {url}");

            var comparisons = Config.GetComparisons();
            foreach (var c in comparisons)
            {
                var uas = c.Split("--");
                try { 
                    var ua1Visit = WebPageVisit.GetVisitByUa(url, uas[0]);
                    var ua2Visit = WebPageVisit.GetVisitByUa(url, uas[1]);
                    if (ua1Visit != null && ua2Visit != null)
                    {
                        Console.WriteLine($"UA change {url} {uas[0]}: { ua1Visit.ID}");
                        Console.WriteLine($"UA change {url} {uas[1]}: { ua2Visit.ID}");
                        await ComputeDomChanges(ua1Visit, ua2Visit, webPage);
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"a problem occurred: { e.Message }");
                    Console.WriteLine($"Error message: { e.Message }");
                    Console.WriteLine($"Error inner exception: { e.InnerException }");
                }
            }
        }
    }
}
