using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary
{
    public class BinaryAddOperator : BinaryPureBase
    {
        public static readonly BinaryPureBase Instance = new BinaryAddOperator();

        private BinaryAddOperator()
        {
        }

        public override string ToString()
        {
            return "+";
        }

        public override (IValue from, IValue to) Apply(IValue value, IValue operand)
        {
            switch (value)
            {
                case OrderedLiteralBase orderedLiteralValue:
                {
                    switch (operand)
                    {
                        case OrderedLiteralBase orderedLiteralOperand:
                            var literal = orderedLiteralValue + orderedLiteralOperand;
                            return (literal, literal);

                        case StringLiteral _:
                            return (null, null);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(operand));
                    }
                }

                case StringLiteral _:
                    return (null, null);

                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}