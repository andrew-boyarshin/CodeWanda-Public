using System.Collections.Generic;
using CodeWanda.Model.Semantic.Expressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Statements
{
    public abstract class SimpleSelectionStatement : StatementBase
    {
        public SimpleSelectionStatement(SimpleCompoundStatement parent)
        {
            Block.ParentBlock = parent;
            Block.ParentMethod = parent.ParentMethod;
        }

        protected SimpleSelectionStatement()
        {
        }

        [NotNull] [ItemNotNull] public List<ExpressionBase> Condition { get; } = new List<ExpressionBase>();
        [NotNull] public SimpleCompoundStatement Block { get; } = new SimpleCompoundStatement();
    }
}