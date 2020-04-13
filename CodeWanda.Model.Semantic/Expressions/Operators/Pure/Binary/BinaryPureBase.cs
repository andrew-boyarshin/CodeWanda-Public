using CodeWanda.Model.Semantic.Data;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary
{
    public abstract class BinaryPureBase : PureBase
    {
        public abstract (IValue from, IValue to) Apply(IValue value, IValue operand);
    }
}