using CodeWanda.Model.Semantic.Data;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Unary
{
    public abstract class UnaryPureBase : PureBase
    {
        public abstract (IValue from, IValue to) Apply(IValue value);
    }
}