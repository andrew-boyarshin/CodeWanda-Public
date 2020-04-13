using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Unary
{
    public class UnaryMinusOperator : UnaryPureBase
    {
        public override string ToString()
        {
            return "-";
        }

        public override (IValue from, IValue to) Apply(IValue value)
        {
            switch (value)
            {
                case OrderedLiteralBase orderedLiteralBase:
                {
                    return (-orderedLiteralBase, -orderedLiteralBase);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}