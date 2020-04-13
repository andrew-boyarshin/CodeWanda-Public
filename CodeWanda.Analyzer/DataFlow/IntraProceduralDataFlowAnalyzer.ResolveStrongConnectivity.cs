using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.StrongConnectivity;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private void ResolveStrongConnectivity()
        {
            var stack = new Stack<GraphNode>();
            foreach (var graphNode in MethodRoot.Start.IterateGraphNodesRecursive(true))
            {
                switch (graphNode)
                {
                    case GraphDataFlowConditionalNode graphConditionalNode:
                        KosarajuVisit(graphConditionalNode, stack);
                        break;
                    case GraphDataFlowReturnNode graphReturnNode:
                        KosarajuVisit(graphReturnNode, stack);
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        KosarajuVisit(graphDataFlowBlockNode, stack);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }

            foreach (var graphNode in stack)
            {
                switch (graphNode)
                {
                    case GraphDataFlowConditionalNode graphConditionalNode:
                        KosarajuAssign(graphConditionalNode, graphConditionalNode);
                        break;
                    case GraphDataFlowReturnNode graphReturnNode:
                        KosarajuAssign(graphReturnNode, graphReturnNode);
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        KosarajuAssign(graphDataFlowBlockNode, graphDataFlowBlockNode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(graphNode));
                }
            }

            var elements = MethodRoot.AllGroups.FindAll(x => x.Nodes.Count < 2);
            foreach (var componentNodeGroup in elements)
            {
                var node = componentNodeGroup.Nodes[0];
                switch (node)
                {
                    case GraphDataFlowConditionalNode graphConditionalNode:
                        graphConditionalNode.StrongConnectivityNode.StrongConnectivityComponent = null;
                        break;
                    case GraphDataFlowReturnNode graphReturnNode:
                        graphReturnNode.StrongConnectivityNode.StrongConnectivityComponent = null;
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        graphDataFlowBlockNode.StrongConnectivityNode.StrongConnectivityComponent = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(node));
                }

                MethodRoot.AllGroups.Remove(componentNodeGroup);
            }
        }

        private void KosarajuVisit<T>(T node, Stack<GraphNode> stack)
            where T : GraphNode, IComponentNodeItem<T>
        {
            if (node.StrongConnectivityNode.Visited)
                return;

            node.StrongConnectivityNode.Visited = true;
            foreach (var childNode in node.AllOutgoing)
            {
                switch (childNode)
                {
                    case GraphDataFlowConditionalNode graphConditionalNode:
                        KosarajuVisit(graphConditionalNode, stack);
                        break;
                    case GraphDataFlowReturnNode graphReturnNode:
                        KosarajuVisit(graphReturnNode, stack);
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        KosarajuVisit(graphDataFlowBlockNode, stack);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            stack.Push(node.StrongConnectivityNode.GraphNode);
        }

        private void KosarajuAssign<T1, T2>(T1 node, T2 root)
            where T1 : GraphNode, IComponentNodeItem<T1>
            where T2 : GraphNode, IComponentNodeItem<T2>
        {
            if (node.StrongConnectivityNode.StrongConnectivityComponent != null)
                return;

            if (root.StrongConnectivityNode.StrongConnectivityComponent == null)
            {
                root.StrongConnectivityNode.StrongConnectivityComponent =
                    new ComponentNodeGroup(root.StrongConnectivityNode.GraphNode);
            }

            node.StrongConnectivityNode.StrongConnectivityComponent =
                root.StrongConnectivityNode.StrongConnectivityComponent;

            root.StrongConnectivityNode.StrongConnectivityComponent.Add(node.StrongConnectivityNode.GraphNode);

            foreach (var graphEdge in node.Incoming)
            {
                switch (graphEdge)
                {
                    case GraphDataFlowConditionalNode graphConditionalNode:
                        KosarajuAssign(graphConditionalNode, root);
                        break;
                    case GraphDataFlowReturnNode graphReturnNode:
                        KosarajuAssign(graphReturnNode, root);
                        break;
                    case GraphDataFlowBlockNode graphDataFlowBlockNode:
                        KosarajuAssign(graphDataFlowBlockNode, root);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}