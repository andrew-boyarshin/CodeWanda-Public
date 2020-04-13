using System.Collections.Generic;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Statements
{
    public class ForIterationStatement : WhileIterationStatement
    {
        [NotNull] [ItemNotNull] public List<ExpressionBase> IterationExpression { get; } = new List<ExpressionBase>();

        public ForIterationStatement(SimpleCompoundStatement parent) : base(parent)
        {
        }

        public override string ToString()
        {
            return $"for (; {string.Join(", ", Condition)}; {string.Join(", ", IterationExpression)})\n{Block}";
        }
    }
}