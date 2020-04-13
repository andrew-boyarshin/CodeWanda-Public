using System;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.ControlFlow
{
    public sealed class GraphControlFlowExpressionNode : GraphExpressionNodeBase
    {
        public GraphControlFlowExpressionNode(
            [NotNull] GraphMethodRoot methodRoot,
            [NotNull] ExpressionBase expression
        ) : base(methodRoot, expression)
        {
        }

        public override void ReplaceOutgoingNode(GraphNode oldNode, GraphNode newNode) =>
            throw new InvalidOperationException();
    }
}