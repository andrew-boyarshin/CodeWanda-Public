using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.StrongConnectivity;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public sealed class GraphDataFlowConditionalNode : GraphDataFlowExpressionNodeBase, IGraphConditionalNode,
        IComponentNodeItem<GraphDataFlowConditionalNode>
    {
        private GraphNode _trueOutgoing;
        private GraphNode _falseOutgoing;

        public GraphNode TrueOutgoing
        {
            get => GetOutgoingNode(_trueOutgoing);
            set => UpdateOutgoingNode(ref _trueOutgoing, value);
        }

        public GraphNode FalseOutgoing
        {
            get => GetOutgoingNode(_falseOutgoing);
            set => UpdateOutgoingNode(ref _falseOutgoing, value);
        }

        [NotNull] public AnalysisContext TrueExitAnalysisContext { get; }
        [NotNull] public AnalysisContext FalseExitAnalysisContext { get; }

        public ComponentNode<GraphDataFlowConditionalNode> StrongConnectivityNode { get; }

        public GraphDataFlowConditionalNode(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression) : base(
            methodRoot, expression)
        {
            StrongConnectivityNode = new ComponentNode<GraphDataFlowConditionalNode>(this);
            TrueExitAnalysisContext = new AnalysisContext(EnterAnalysisContext);
            FalseExitAnalysisContext = new AnalysisContext(EnterAnalysisContext);
            
            ((IComponentNodeItem<GraphDataFlowConditionalNode>) this).SubscribeToStrongConnectivityNodePropertyChanged();
            SubscribeToAnalysisContextChanges(TrueExitAnalysisContext);
            SubscribeToAnalysisContextChanges(FalseExitAnalysisContext);
            
            RefreshText();
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            ((IGraphConditionalNode) this).ReplaceOutgoingNodeImpl(oldNode, newNode);
    }
}