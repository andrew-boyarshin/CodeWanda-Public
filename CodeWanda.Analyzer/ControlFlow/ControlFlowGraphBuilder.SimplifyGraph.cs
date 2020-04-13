using System;
using System.Linq;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using Serilog;

namespace CodeWanda.Analyzer.ControlFlow
{
    public partial class ControlFlowGraphBuilder
    {
        private bool SimplifyGraph(GraphControlFlowBlockNode blockNode)
        {
            if (blockNode.Body.Count != 0) return false;

            var child = blockNode.Outgoing;

            child.ReplaceIncomingNode(blockNode, blockNode.Incoming, true);

            foreach (var parentNode in blockNode.Incoming)
                parentNode.ReplaceOutgoingNode(blockNode, child);

            blockNode.Incoming.Clear();

            return true;
        }

        private bool SimplifyGraph(GraphMethodRoot methodNode)
        {
            var oldStart = methodNode.Start;

            var res = SimplifyNode(oldStart);

            if (res)
            {
                if (oldStart is GraphControlFlowBlockNode oldStartBlockNode)
                {
                    methodNode.Start = oldStartBlockNode.Outgoing;
                    Log.Verbose(
                        "CFG: {MethodName}: simplified away start point {Node}, new start point is {NewNode}",
                        methodNode.Name,
                        oldStart,
                        methodNode.Start
                    );
                }
                else
                {
                    Log.Error(
                        "CFG: {MethodName}: simplified away start point {Node}, but it wasn't {TypeName}",
                        methodNode.Name,
                        oldStart,
                        nameof(GraphControlFlowBlockNode)
                    );
                }
            }

            var graphNodes = methodNode.Start.IterateGraphNodesRecursive(false).ToArray();

            foreach (var graphNode in graphNodes)
            {
                res = SimplifyNode(graphNode) || res;
            }

            return res;

            bool SimplifyNode(GraphNode graphNode)
            {
                return graphNode switch
                {
                    null => throw new ArgumentOutOfRangeException(nameof(graphNode)),
                    GraphControlFlowBlockNode blockNode => SimplifyGraph(blockNode),
                    GraphControlFlowConditionalNode _ => false,
                    GraphControlFlowReturnNode _ => false,
                    _ => throw new ArgumentOutOfRangeException(nameof(graphNode))
                };
            }
        }
    }
}