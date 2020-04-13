using System;
using System.Linq;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.ControlFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;

namespace CodeWanda.Analyzer.DataFlow
{
    public class DataFlowGraphBuilder
    {
        private GraphMethodRoot MethodRoot { get; }

        public DataFlowGraphBuilder(GraphMethodRoot methodRoot)
        {
            MethodRoot = methodRoot;
        }

        public void Convert()
        {
            MethodRoot.Start = ConvertNode(MethodRoot.Start);

            var graphNodes = MethodRoot.Start.IterateGraphNodesRecursive(false).ToArray();

            foreach (var graphNode in graphNodes)
            {
                ConvertNode(graphNode);
            }

            GraphNode ConvertNode(GraphNode graphNode)
            {
                return graphNode switch
                {
                    null => throw new ArgumentOutOfRangeException(nameof(graphNode)),
                    GraphDataFlowBlockNode _ => graphNode,
                    GraphDataFlowExpressionNode _ => graphNode,
                    GraphControlFlowBlockNode blockNode => ConvertControlFlowNode(blockNode),
                    GraphControlFlowConditionalNode conditionalNode => ConvertControlFlowNode(conditionalNode),
                    GraphControlFlowReturnNode returnNode => ConvertControlFlowNode(returnNode),
                    GraphDataFlowConditionalNode _ => graphNode,
                    GraphDataFlowReturnNode _ => graphNode,
                    _ => throw new ArgumentOutOfRangeException(nameof(graphNode))
                };
            }
        }

        private GraphDataFlowBlockNode ConvertControlFlowNode(GraphControlFlowBlockNode controlFlowNode)
        {
            var dataFlowNode = new GraphDataFlowBlockNode(MethodRoot);
            
            foreach (var expressionBase in controlFlowNode.Body)
            {
                var expressionNode = new GraphDataFlowExpressionNode(MethodRoot, expressionBase.Expression, dataFlowNode);
                dataFlowNode.Body.Add(expressionNode);
            }

            dataFlowNode.Outgoing = controlFlowNode.Outgoing;
            dataFlowNode.Outgoing?.ReplaceIncomingNode(controlFlowNode, new[] {dataFlowNode}, false);

            return ConvertNodeIncoming(controlFlowNode, dataFlowNode);
        }

        private GraphDataFlowConditionalNode ConvertControlFlowNode(GraphControlFlowConditionalNode controlFlowNode)
        {
            var dataFlowNode = new GraphDataFlowConditionalNode(MethodRoot, controlFlowNode.Expression)
            {
                TrueOutgoing = controlFlowNode.TrueOutgoing,
                FalseOutgoing = controlFlowNode.FalseOutgoing
            };

            dataFlowNode.TrueOutgoing?.ReplaceIncomingNode(controlFlowNode, new[] {dataFlowNode}, false);
            dataFlowNode.FalseOutgoing?.ReplaceIncomingNode(controlFlowNode, new[] {dataFlowNode}, false);

            return ConvertNodeIncoming(controlFlowNode, dataFlowNode);
        }

        private GraphDataFlowReturnNode ConvertControlFlowNode(GraphControlFlowReturnNode controlFlowNode)
        {
            var dataFlowNode = new GraphDataFlowReturnNode(MethodRoot, controlFlowNode.Expression);

            return ConvertNodeIncoming(controlFlowNode, dataFlowNode);
        }

        private static T ConvertNodeIncoming<T>(GraphNode controlFlowNode, T dataFlowNode) where T: GraphNode
        {
            foreach (var parentNode in controlFlowNode.Incoming)
            {
                parentNode.ReplaceOutgoingNode(controlFlowNode, dataFlowNode);

                dataFlowNode.Incoming.Add(parentNode);
            }

            controlFlowNode.Incoming.Clear();

            return dataFlowNode;
        }
    }
}