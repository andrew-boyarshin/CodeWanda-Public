using System;
using CodeWanda.Model.Graph.StrongConnectivity;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public sealed class GraphDataFlowReturnNode
        : GraphDataFlowExpressionNodeBase, IComponentNodeItem<GraphDataFlowReturnNode>
    {
        public ComponentNode<GraphDataFlowReturnNode> StrongConnectivityNode { get; }

        public GraphDataFlowReturnNode(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression) : base(
            methodRoot, expression)
        {
            StrongConnectivityNode = new ComponentNode<GraphDataFlowReturnNode>(this);
            ((IComponentNodeItem<GraphDataFlowReturnNode>) this).SubscribeToStrongConnectivityNodePropertyChanged();

            RefreshText();
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            throw new InvalidOperationException();
    }
}