using System;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.StrongConnectivity;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private void ProcessNode([NotNull] GraphNode current,
            [NotNull] GraphNode[] path,
            [CanBeNull] ComponentNodeGroup targetGroup = null)
        {
            if (current == null) throw new ArgumentNullException(nameof(current));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (path.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(path));

            if (!_nodes.Add(current))
                return;

            ComponentNodeGroup componentNodeGroup = null;
            switch (current)
            {
                case GraphDataFlowConditionalNode graphConditionalNode:
                    componentNodeGroup = graphConditionalNode.StrongConnectivityNode.StrongConnectivityComponent;
                    break;
                case GraphDataFlowReturnNode graphReturnNode:
                    componentNodeGroup = graphReturnNode.StrongConnectivityNode.StrongConnectivityComponent;
                    break;
                case GraphDataFlowBlockNode graphDataFlowBlockNode:
                    componentNodeGroup = graphDataFlowBlockNode.StrongConnectivityNode.StrongConnectivityComponent;
                    break;
            }

            if (componentNodeGroup != null && componentNodeGroup.RootIncomingPath == null)
            {
                if (componentNodeGroup.Root == current)
                {
                    componentNodeGroup.RootIncomingPath = path;
                }
            }

            switch (current)
            {
                case GraphDataFlowBlockNode graphBlockNode:
                {
                    foreach (var expression in graphBlockNode.Body)
                    {
                        ProcessExpression(expression, path);
                    }

                    var outgoingTo = graphBlockNode.Outgoing;
                    if (outgoingTo != null && (targetGroup == null || targetGroup.Nodes.Contains(outgoingTo)))
                    {
                        ProcessNode(outgoingTo,
                            path.Concat(new[] {outgoingTo}).ToArray());
                    }

                    break;
                }

                case GraphDataFlowConditionalNode graphConditionalNode:
                {
                    switch (graphConditionalNode.Expression)
                    {
                        case BinaryLogicExpression binaryLogicExpression:
                            SolveConditionContexts(graphConditionalNode, binaryLogicExpression, path);
                            break;
                        case UnaryLogicExpression unaryLogicExpression:
                            SolveConditionContexts(graphConditionalNode, unaryLogicExpression, path);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var trueOutgoingTo = graphConditionalNode.TrueOutgoing;
                    if (trueOutgoingTo != null && (targetGroup == null || targetGroup.Nodes.Contains(trueOutgoingTo)))
                    {
                        ProcessNode(trueOutgoingTo,
                            path.Concat(new[] {trueOutgoingTo}).ToArray());
                    }

                    var falseOutgoingTo = graphConditionalNode.FalseOutgoing;
                    if (falseOutgoingTo != null && (targetGroup == null || targetGroup.Nodes.Contains(falseOutgoingTo)))
                    {
                        ProcessNode(falseOutgoingTo,
                            path.Concat(new[] {falseOutgoingTo}).ToArray());
                    }

                    break;
                }

                case GraphDataFlowReturnNode _:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(current));
            }
        }
    }
}