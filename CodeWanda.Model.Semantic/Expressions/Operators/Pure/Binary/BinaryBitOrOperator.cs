using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary
{
    public class BinaryBitOrOperator : BinaryPureBase
    {
        public static readonly BinaryBitOrOperator Instance = new BinaryBitOrOperator();

        private BinaryBitOrOperator()
        {
        }

        public override string ToString()
        {
            return "|";
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
                            var literal = orderedLiteralValue | orderedLiteralOperand;
                            return (literal, literal);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(operand));
                    }
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}