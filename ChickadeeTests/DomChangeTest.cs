using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chickadee;
using tree_matching_csharp;
using System.Linq;
using System.Collections.Generic;

namespace ChickadeeTests
{
    [TestClass]
    public class DomChangeTest
    {
        [TestMethod]
        public void TestSaveDomChange()
        {
            using (var context = new LibraryContext())
            {
                var url = "example.com";
                var webPage = WebPage.SaveWebPage(url);
                var savedWebPage = context.WebPage.Where(w => w.Url.Equals(url));

                var expectedCount = 1;
                var actualCount = savedWebPage.Count<WebPage>();
                Assert.AreEqual(expectedCount, actualCount);

                var configUid = "Generic";
                var webPageVisit = WebPageVisit.SaveWebPageVisit(webPage, configUid);
                var savedWebPageVisit = context.WebPageVisit.Where(v => v.ID.Equals(webPageVisit.ID));
                expectedCount = 1;
                actualCount = savedWebPageVisit.Count<WebPageVisit>();
                Assert.AreEqual(expectedCount, actualCount);

                var originalNode = new Node
                {
                    NodeSignature = "HEAD"
                };
                var instrumentedNode = new Node
                {
                    NodeSignature = "HEAD"
                };
                var edge = new Edge
                {
                    Source = originalNode,
                    Target = instrumentedNode
                };

                var domStatistics = DomChange.BuildDomStatistics((IEnumerable<Edge>)edge);
                var domChange = DomChange.SaveDomChange(domStatistics, webPageVisit);
                var savedDomChange = context.DomChange.Where(d => d.WebPageVisit.Equals(webPageVisit));

                Assert.AreEqual(1, savedDomChange.Count<DomChange>());

                context.DomChange.RemoveRange(savedDomChange);
                context.WebPageVisit.RemoveRange(savedWebPageVisit);
                context.WebPage.RemoveRange(savedWebPage);
                context.SaveChanges();
            }
        }
    }
}
