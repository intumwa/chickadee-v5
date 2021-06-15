using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tree_matching_csharp;

namespace Chickadee
{
    public static class WebPageMatcher
    {
        public static SftmTreeMatcher.Parameters _parameters = new SftmTreeMatcher.Parameters
        {
            LimitNeighbors = 100,
            MetropolisParameters = new Metropolis.Parameters
            {
                Gamma = 0.9f,
                Lambda = 0.7f,
                NbIterations = 10,
            },
            NoMatchCost = 1.2,
            MaxPenalizationChildren = 0.4,
            MaxPenalizationParentsChildren = 0.2,
            PropagationParameters = new SimilarityPropagation.Parameters()
            {
                Envelop = new[] { 0.8, 0.1, 0.01 },
                // Envelop    = new[] {0.0},
                Parent = 0.25,
                Sibling = 0.0,
                SiblingInv = 0.0,
                ParentInv = 0.7,
                Children = 0.1
            },
            MaxTokenAppearance = n => (int)Math.Sqrt(n)
        };

        public static async Task<TreeMatcherResponse> MatchDomTrees(IEnumerable<Node> sourceTree, IEnumerable<Node> targetTree)
        {
            var domTreeMatcher = new SftmTreeMatcher(_parameters);
            var matcherResult = await domTreeMatcher.MatchTrees(sourceTree, targetTree);

            return matcherResult;
        }
        
        public static int[] CountNodes(Edge edge)
        {
            int originalNodeCount = 0;
            if (edge.Source != null)
                originalNodeCount++;

            int instrumentedNodeCount = 0;
            if (edge.Target != null)
                instrumentedNodeCount++;

            int[] count = { originalNodeCount, instrumentedNodeCount };

            return count;
        }

        public static string[] FormatNode(Edge edge)
        {
            string originalNodeSignature;
            if (edge.Source != null)
                originalNodeSignature = edge.Source.NodeSignature;
            else
                originalNodeSignature = "-";

            string instrumentedNodeSignature;
            if (edge.Target != null)
                instrumentedNodeSignature = edge.Target.NodeSignature;
            else
                instrumentedNodeSignature = "-";

            string[] format = { originalNodeSignature, instrumentedNodeSignature };

            return format;
        }

        public static string[] FormatTags(Edge edge)
        {
            string originalTag;
            if (edge.Source != null)
                originalTag = edge.Source.Value[0];
            else
                originalTag = "-";

            string instrumentedTag;
            if (edge.Target != null)
                instrumentedTag = edge.Target.Value[0];
            else
                instrumentedTag = "-";

            string[] format = { originalTag, instrumentedTag };

            return format;
        }

        public static ChangeType CheckNodeChange(Edge edge)
        {
            var nodeFormat = FormatNode(edge);
            var originalNodeSignature = nodeFormat[0];
            var instrumentedNodeSignature = nodeFormat[1];

            ChangeType change;
            if (originalNodeSignature.Equals(instrumentedNodeSignature))
                change = ChangeType.Unchanged;
            else if (!originalNodeSignature.Equals("-") && instrumentedNodeSignature.Equals("-"))
                change = ChangeType.Deleted;
            else if (originalNodeSignature.Equals("-") && !instrumentedNodeSignature.Equals("-"))
                change = ChangeType.Inserted;
            else
                change = ChangeType.Changed;

            return change;
        }
    }
}
