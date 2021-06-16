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
            Console.WriteLine();
            Console.WriteLine($"url id: {webPage.ID}, {url}");

            var generic = Config.GetGeneric();
            var configs = Config.GetConfigs(generic);

            Console.WriteLine($"generic is {generic.UID}");
            Console.WriteLine($"comparisons length: {configs.Count}");
            Console.WriteLine();

            foreach (var c in configs)
            {
                try { 
                    var ua1Visit = WebPageVisit.GetVisitByUa(url, generic.UID);
                    var ua2Visit = WebPageVisit.GetVisitByUa(url, c.UID);
                    if (ua1Visit != null && ua2Visit != null)
                    {
                        Console.WriteLine($"UA change {url} {generic.UID}: { ua1Visit.ID}");
                        Console.WriteLine($"UA change {url} {c.UID}: { ua2Visit.ID}");
                        await ComputeDomChanges(ua1Visit, ua2Visit, webPage);
                    } else {
                        Console.WriteLine($"couldn't work out {generic.UID} {c.UID}");
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
