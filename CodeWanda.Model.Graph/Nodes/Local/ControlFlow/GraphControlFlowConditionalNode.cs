using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.ControlFlow
{
    public sealed class GraphControlFlowConditionalNode : GraphExpressionNodeBase, IGraphConditionalNode
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

        public GraphControlFlowConditionalNode(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression)
            : base(methodRoot, expression)
        {
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            ((IGraphConditionalNode) this).ReplaceOutgoingNodeImpl(oldNode, newNode);
    }
}