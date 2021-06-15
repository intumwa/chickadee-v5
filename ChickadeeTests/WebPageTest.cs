using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chickadee;
using System;
using tree_matching_csharp;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ChickadeeTests
{
    [TestClass]
    public class WebPageTest
    {
        [TestMethod]
        public void TestSaveWebPage()
        {
            var url = "example.com";
            WebPage.SaveWebPage(url);

            using (var context = new LibraryContext())
            {
                var webPage = context.WebPage.Where(w => w.Url.Equals(url));
                var expectedCount = 1;
                var actualCount = webPage.Count<WebPage>();

                Assert.AreEqual(expectedCount, actualCount);

                context.WebPage.RemoveRange(webPage);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void TestBuildFilePath()
        {
            var expectedFilePath = @"C:\Users\user\source\repos\Scrapper\Scrapper\webpages\example.com\original.html";
            var actualFilePath = FileHelper.BuildFilePath("example.com", "original.html");
            Assert.AreEqual(expectedFilePath, actualFilePath);
        }

        [TestMethod]
        public void TestRenameFile()
        {
            var expectedFilePath = Path.Join("C:", "Users", "user", "source", "repos", "Scrapper", "Scrapper", "webpages", "example.com");
            var actualFilePath = FileHelper.RenameFile("example.com", "original.html");
            Assert.IsTrue(actualFilePath.Contains(expectedFilePath));

            FileSystem.RenameFile(actualFilePath, "original.html");
        }

        [TestMethod]
        public async Task TestGetDomTree()
        {
            var expectedFile = "original.html";
            var actualFile = "instrumented.html";

            var expectedTreeTask = WebPage.GetDomTree(expectedFile);
            var actualTreeTreeTask = WebPage.GetDomTree(actualFile);

            var expectedTree = await expectedTreeTask;
            var actualTree = await actualTreeTreeTask;

            Assert.AreEqual(expectedTree.Count<Node>(), actualTree.Count<Node>());
        }
    }
}
