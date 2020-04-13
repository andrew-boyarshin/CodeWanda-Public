using System.Diagnostics;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public interface IGraphConditionalNode
    {
        public GraphNode TrueOutgoing { get; set; }

        public GraphNode FalseOutgoing { get; set; }

        public void ReplaceOutgoingNodeImpl(GraphNode oldNode, GraphNode newNode)
        {
            Debug.Assert(TrueOutgoing == oldNode || FalseOutgoing == oldNode);
            if (TrueOutgoing == oldNode)
                TrueOutgoing = newNode;
            if (FalseOutgoing == oldNode)
                FalseOutgoing = newNode;
        }
    }
}