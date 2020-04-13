using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;

namespace CodeWanda.Analyzer.ControlFlow
{
    public partial class ControlFlowGraphBuilder
    {
	    private static void CalculateIncomingInternal(GraphNode node)
	    {
		    foreach (var graphNode in node.AllOutgoing)
		    {
                if (graphNode.Incoming.Contains(node))
                    continue;

                graphNode.Incoming.Add(node);

			    CalculateIncomingInternal(graphNode);
		    }
	    }

        private static void CalculateIncoming(GraphNode blockNode)
        {
	        foreach (var graphNode in blockNode.IterateGraphNodesRecursive(true))
	        {
		        graphNode.Incoming.Clear();
	        }

			CalculateIncomingInternal(blockNode);
        }
    }
}