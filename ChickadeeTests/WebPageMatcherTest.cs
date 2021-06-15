using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chickadee;
using tree_matching_csharp;

namespace ChickadeeTests
{
    [TestClass]
    public class WebPageMatcherTest
    {
        [TestMethod]
        public void TestCheckNodeChange()
        {
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
            ChangeType change = WebPageMatcher.CheckNodeChange(edge);
            Assert.IsTrue(change.Equals(ChangeType.Unchanged));

            originalNode = new Node
            {
                NodeSignature = "DIV"
            };
            edge = new Edge
            {
                Source = originalNode,
                Target = null
            };
            change = WebPageMatcher.CheckNodeChange(edge);
            Assert.IsTrue(change.Equals(ChangeType.Deleted));

            instrumentedNode = new Node
            {
                NodeSignature = "DIV"
            };
            edge = new Edge
            {
                Source = null,
                Target = instrumentedNode
            };
            change = WebPageMatcher.CheckNodeChange(edge);
            Assert.IsTrue(change.Equals(ChangeType.Inserted));

            originalNode = new Node
            {
                NodeSignature = "H1"
            };
            instrumentedNode = new Node
            {
                NodeSignature = "H3"
            };
            edge = new Edge
            {
                Source = originalNode,
                Target = instrumentedNode
            };
            change = WebPageMatcher.CheckNodeChange(edge);
            Assert.IsTrue(change.Equals(ChangeType.Changed));
        }
    }
}
