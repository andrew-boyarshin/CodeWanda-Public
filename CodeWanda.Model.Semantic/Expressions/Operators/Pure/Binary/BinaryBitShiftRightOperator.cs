using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary
{
    public class BinaryBitShiftRightOperator : BinaryPureBase
    {
        public static readonly BinaryBitShiftRightOperator Instance = new BinaryBitShiftRightOperator();

        private BinaryBitShiftRightOperator()
        {
        }

        public override string ToString()
        {
            return ">>";
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
                            var literal = OrderedLiteralBase.BitRightShift(orderedLiteralValue, orderedLiteralOperand);
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