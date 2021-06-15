using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chickadee;
using System.Linq;

namespace ChickadeeTests
{
    [TestClass]
    public class WebPageVisitTest
    {
        [TestMethod]
        public void TestSaveWebPageVisit()
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

                context.WebPageVisit.RemoveRange(savedWebPageVisit);
                context.WebPage.RemoveRange(savedWebPage);
                context.SaveChanges();
            } 
        }
    }
}
