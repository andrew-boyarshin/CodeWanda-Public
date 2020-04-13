using System;
using System.Diagnostics;
using System.Linq;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using Serilog;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private static void AssignParentsToContexts(GraphDataFlowExpressionNodeBase currentNode, GraphNode currentBlockNode, GraphNode parentNode)
        {
            switch (parentNode)
            {
                case null:
                    throw new ArgumentNullException(nameof(parentNode));
                case GraphControlFlowBlockNode _:
                    goto default;
                case GraphDataFlowConditionalNode graphConditionalNode:
                    if (currentBlockNode == graphConditionalNode.TrueOutgoing)
                        currentNode.EnterAnalysisContext.AddParent(graphConditionalNode.TrueExitAnalysisContext);
                    else if (currentBlockNode == graphConditionalNode.FalseOutgoing)
                        currentNode.EnterAnalysisContext.AddParent(graphConditionalNode.FalseExitAnalysisContext);
                    else
                        Log.Error("Edge {From}->{To} is invalid", parentNode, currentNode);

                    break;
                case GraphDataFlowReturnNode _:
                    goto default;
                case GraphDataFlowBlockNode graphDataFlowBlockNode:
                {
                    Debug.Assert(currentBlockNode == graphDataFlowBlockNode.Outgoing);
                    var last = graphDataFlowBlockNode.Body.Last();
                    currentNode.EnterAnalysisContext.AddParent(last.ExitAnalysisContext);
                    break;
                }

                case GraphDataFlowExpressionNode _:
                    goto default;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parentNode));
            }
        }

        private void AssignParentsToContexts(GraphMethodRoot methodNode)
        {
            var graphNodes = methodNode.Start.IterateGraphNodesRecursive(true).ToArray();
            foreach (var graphNode in graphNodes)
            {
                switch (graphNode)
                {
                    case null:
                        throw new ArgumentNullException(nameof(graphNode));
                    case GraphDataFlowExpressionNodeBase dataFlowExpressionNode:
                        foreach (var parentNode in dataFlowExpressionNode.Incoming)
                        {
                            AssignParentsToContexts(dataFlowExpressionNode, dataFlowExpressionNode, parentNode);
                        }

                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        if (graphDataFlowBlockNode.Body.Count > 0)
                        {
                            var firstExpressionNode = graphDataFlowBlockNode.Body[0];

                            foreach (var parentNode in graphDataFlowBlockNode.Incoming)
                            {
                                AssignParentsToContexts(firstExpressionNode, graphDataFlowBlockNode, parentNode);
                            }

                            for (var i = 1; i < graphDataFlowBlockNode.Body.Count; i++)
                            {
                                var previous = graphDataFlowBlockNode.Body[i - 1];
                                var current = graphDataFlowBlockNode.Body[i];
                                current.EnterAnalysisContext.AddParent(previous.ExitAnalysisContext);
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }
        }
    }
}