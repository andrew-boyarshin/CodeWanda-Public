using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary
{
    public class BinaryRemainderOperator : BinaryPureBase
    {
        public static readonly BinaryRemainderOperator Instance = new BinaryRemainderOperator();

        private BinaryRemainderOperator()
        {
        }

        public override string ToString()
        {
            return "%";
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
                            var literal = orderedLiteralValue % orderedLiteralOperand;
                            return (literal, literal);

                        default:
                            throw new ArgumentOutOfRangeException(nameof(operand));
                    }
                }

                case { }:
                {
                    switch (operand)
                    {
                        case IntegerLiteral integerLiteral:
                            var literalFrom = IntegerLiteral.Zero;
                            OrderedLiteralBase literalTo = new IntegerLiteral(integerLiteral);
                            if (integerLiteral.AbsoluteValue > 0)
                                literalTo -= IntegerLiteral.One;
                            return (literalFrom, literalTo);
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