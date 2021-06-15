using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using tree_matching_csharp;

namespace Chickadee
{
    public class DomChange
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Ua1 { get; set; }
        public string Ua2 { get; set; }
        public int Ua1NodesCount { get; set; }
        public int Ua2NodesCount { get; set; }
        public string UnchangedNodes { get; set; }
        public int UnchangedNodesCount { get; set; }
        public string ChangedNodes { get; set; }
        public int ChangedNodesCount { get; set; }
        public string InsertedNodes { get; set; }
        public int InsertedNodesCount { get; set; }
        public string DeletedNodes { get; set; }
        public int DeletedNodesCount { get; set; }
        public DateTime Ua1VisitTime { get; set; }
        public DateTime Ua2VisitTime { get; set; }
        public virtual WebPageVisit Ua1Visit { get; set; }
        public virtual WebPageVisit Ua2Visit { get; set; }

        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static DomChange SaveDomChange(DomChange change, WebPageVisit ua1Visit, WebPageVisit ua2Visit)
        {
            using (var context = new LibraryContext())
            {
                context.WebPageVisit.Attach(ua1Visit);
                context.WebPageVisit.Attach(ua2Visit);
                try
                {
                    change.Ua1Visit = ua1Visit;
                    change.Ua2Visit = ua2Visit;
                    context.DomChange.Add(change);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    logger.Error($"An error occured while saving DOM changes: { e.Message }");
                    Console.WriteLine($"Error message: { e.Message }");
                    Console.WriteLine($"Error inner exception: { e.InnerException }");
                    change = null;
                }
            }
            return change;
        }

        public static DomChange ProcessDomChange(IEnumerable<Edge> edges, WebPageVisit ua1, WebPageVisit ua2)
        {
            var ua1NodesCount = 0;
            var ua2NodesCount = 0;
            var unchangedNodesCount = 0;
            var changedNodesCount = 0;
            var insertedNodesCount = 0;
            var deletedNodesCount = 0;

            List<string> unchangedNodes = new List<string>();
            List<string> changedNodes = new List<string>();
            List<string> insertedNodes = new List<string>();
            List<string> deletedNodes = new List<string>();

            foreach (var edge in edges)
            {
                var nodesCount = WebPageMatcher.CountNodes(edge);
                ua1NodesCount += nodesCount[0];
                ua2NodesCount += nodesCount[1];

                var nodeChange = WebPageMatcher.CheckNodeChange(edge);
                var tags = WebPageMatcher.FormatTags(edge);
                var ua1VisitTag = tags[0];
                var ua2VisitTag = tags[1];

                if (nodeChange.Equals(ChangeType.Unchanged))
                {
                    if (!unchangedNodes.Contains(ua1VisitTag) && !changedNodes.Contains(ua1VisitTag) && !insertedNodes.Contains(ua2VisitTag) && !deletedNodes.Contains(ua1VisitTag))
                        unchangedNodes.Add(ua1VisitTag);
                    unchangedNodesCount++;
                }
                if (nodeChange.Equals(ChangeType.Changed))
                {
                    if (!changedNodes.Contains(ua1VisitTag))
                    {
                        changedNodes.Add(ua1VisitTag);
                        unchangedNodes.Remove(ua1VisitTag);
                    }
                    changedNodesCount++;
                } 
                if (nodeChange.Equals(ChangeType.Inserted))
                {
                    if (!insertedNodes.Contains(ua2VisitTag))
                    {
                        insertedNodes.Add(ua2VisitTag);
                        unchangedNodes.Remove(ua2VisitTag);
                    }
                    insertedNodesCount++;
                }
                if (nodeChange.Equals(ChangeType.Deleted))
                {
                    if (!deletedNodes.Contains(ua1VisitTag))
                    {
                        deletedNodes.Add(ua1VisitTag);
                        unchangedNodes.Remove(ua1VisitTag);
                    }
                    deletedNodesCount++;
                }

            }

            List<string> newUnchangedNodes = SortUnchangedNodes(unchangedNodes, changedNodes, insertedNodes, deletedNodes);

            DomChange change = new DomChange
            {
                Url = ua1.Url,
                Ua1 = ua1.ConfigurationUid,
                Ua2 = ua2.ConfigurationUid,
                Ua1NodesCount = ua1NodesCount,
                Ua2NodesCount = ua2NodesCount,
                UnchangedNodes = string.Join(",", newUnchangedNodes),
                UnchangedNodesCount = unchangedNodesCount,
                ChangedNodes = string.Join(",", changedNodes),
                ChangedNodesCount = changedNodesCount,
                InsertedNodes = string.Join(",", insertedNodes),
                InsertedNodesCount = insertedNodesCount,
                DeletedNodes = string.Join(",", deletedNodes),
                DeletedNodesCount = deletedNodesCount,
                Ua1VisitTime = ua1.VisitTime,
                Ua2VisitTime = ua2.VisitTime
            };

            return change;
        }

        private static List<string> SortUnchangedNodes(List<string> unchangedNodes, List<string> changedNodes, List<string> insertedNodes, List<string> deletedNodes)
        {
            var list1 = unchangedNodes.Except(changedNodes).ToList();
            var list2 = list1.Except(insertedNodes).ToList();            
            var newUnchangedNodes = list2.Except(deletedNodes).ToList();

            return newUnchangedNodes;
        }

        public static DomChange GetDomChange(WebPageVisit ua1, WebPageVisit ua2)
        {
            DomChange dom;
            using (var context = new LibraryContext())
            {
                try
                {
                    Console.WriteLine($"double-checking: {ua1.ID} {ua2.ID}");
                    dom = context.DomChange.Where(d => d.Ua1Visit.Equals(ua1)).Where(d => d.Ua2Visit.Equals(ua2)).FirstOrDefault();
                }
                catch (Exception e)
                {
                    logger.Error($"Couldn' find DOM change for {ua1.Url} {ua1.ConfigurationUid} {ua2.ConfigurationUid}): { e.Message }");
                    Console.WriteLine($"Couldn' find DOM change for {ua1.Url} {ua1.ConfigurationUid} {ua2.ConfigurationUid}): { e.Message }");
                    Console.WriteLine($"Error inner exception: { e.InnerException }");
                    dom = null;
                }
            }
            return dom;
        }
    }
}
