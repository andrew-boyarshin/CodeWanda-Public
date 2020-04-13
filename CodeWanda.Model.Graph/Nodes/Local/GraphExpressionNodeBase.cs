using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local
{
    public abstract class GraphExpressionNodeBase : GraphNode
    {
        [CanBeNull] public ExpressionBase Expression { get; }

        protected GraphExpressionNodeBase(GraphMethodRoot methodRoot, [CanBeNull] ExpressionBase expression)
            : base(methodRoot)
        {
            Expression = expression;

            RefreshText();
        }

        public override string ToString()
        {
            return Expression?.ToString() ?? string.Empty;
        }
    }
}