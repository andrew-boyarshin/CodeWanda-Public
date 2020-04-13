using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public abstract class LogicBase : ExpressionBase, IPureValue
    {
        [CanBeNull]
        public abstract LogicBase Flip();

        [CanBeNull]
        public abstract LogicBase Negate();
    }
}