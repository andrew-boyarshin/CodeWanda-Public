using CodeWanda.Model.Graph.StrongConnectivity;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public sealed class GraphDataFlowBlockNode
        : GraphBlockNode<GraphDataFlowExpressionNode>, IComponentNodeItem<GraphDataFlowBlockNode>
    {
        public ComponentNode<GraphDataFlowBlockNode> StrongConnectivityNode { get; }

        public GraphDataFlowBlockNode([NotNull] GraphMethodRoot methodRoot) : base(methodRoot)
        {
            StrongConnectivityNode = new ComponentNode<GraphDataFlowBlockNode>(this);
            ((IComponentNodeItem<GraphDataFlowBlockNode>) this).SubscribeToStrongConnectivityNodePropertyChanged();

            RefreshText();
        }
    }
}