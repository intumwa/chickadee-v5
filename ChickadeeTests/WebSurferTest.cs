using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chickadee;
using tree_matching_csharp;
using System.Threading.Tasks;
using System.Linq;

namespace ChickadeeTests
{
    [TestClass]
    public class WebSurferTest
    {
        [TestMethod]
        public async Task TestGetWebPageDocument()
        {
            var expectedDocumentTask = WebSurfer.GetWebPageDocument(@"C:\Users\user\source\repos\Scrapper\Scrapper\webpages\example.com\original.html");
            var actualDocumentTask = WebSurfer.GetWebPageDocument(@"C:\Users\user\source\repos\Scrapper\Scrapper\webpages\example.com\instrumented.html");

            var expectedDocument = await expectedDocumentTask;
            var actualDocument = await actualDocumentTask;

            Assert.AreEqual(expectedDocument.ChildElementCount, actualDocument.ChildElementCount);
        }

        [TestMethod]
        public async Task TestGetWebPageDomTree()
        {
            var expectedFilePath = @"C:\Users\user\source\repos\Scrapper\Scrapper\webpages\example.com\original.html";
            var actualFilePath = @"C:\Users\user\source\repos\Scrapper\Scrapper\webpages\example.com\instrumented.html";

            var expectedDocumentTask = WebSurfer.GetWebPageDocument(expectedFilePath);
            var actualDocumentTask = WebSurfer.GetWebPageDocument(actualFilePath);

            var expectedDocument = await expectedDocumentTask;
            var actualDocument = await actualDocumentTask;

            var expectedDomTree = WebSurfer.GetWebPageDomTree(expectedDocument);
            var actualDomTree = WebSurfer.GetWebPageDomTree(actualDocument);

            Assert.AreEqual(expectedDomTree.Count<Node>(), actualDomTree.Count<Node>());
        }
    }
}
