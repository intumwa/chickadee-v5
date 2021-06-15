using System;
using System.Collections;
using System.Collections.Generic;

namespace tree_matching_csharp
{
    public class Edge
    {
        public Node   Source;
        public Node   Target;
        public FtmCost.Cost FtmCost;
        public double NormalizedScore;
        public double Score { get; set; }
    }

    public class Node
    {
        public Node()
        {
            Id = Guid.NewGuid();
            Children = new List<Node>();
        }

        public Guid Id;
        public List<string> Value;

        //variable added to have a different signature format for my testing purpose
        public string NodeSignature;

        public string Signature;
        public Node Parent;
        public Node LeftSibling;
        public IList<Node> Children;

        public static implicit operator Node(List<string> v)
        {
            throw new NotImplementedException();
        }
    }
}