using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public abstract class OrderedLiteralBase : LiteralBase, IComparable<OrderedLiteralBase>,
        IEquatable<OrderedLiteralBase>
    {
        public abstract int CompareTo(OrderedLiteralBase other);
        public abstract bool Equals(OrderedLiteralBase other);

        public static (OrderedLiteralBase, OrderedLiteralBase) LiftToCommonType([NotNull] OrderedLiteralBase a,
            [NotNull] OrderedLiteralBase b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            switch (a)
            {
                case CharLiteral charLiteral:
                    switch (b)
                    {
                        case CharLiteral charLiteral1:
                            return (charLiteral, charLiteral1);
                        case FloatLiteral floatLiteral:
                            return (new FloatLiteral(charLiteral), floatLiteral);
                        case IntegerLiteral integerLiteral:
                            return (new IntegerLiteral(charLiteral), integerLiteral);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(b));
                    }

                case FloatLiteral floatLiteral:
                    switch (b)
                    {
                        case CharLiteral charLiteral:
                            return (floatLiteral, new FloatLiteral(charLiteral));
                        case FloatLiteral floatLiteral1:
                            return (floatLiteral, floatLiteral1);
                        case IntegerLiteral integerLiteral:
                            return (floatLiteral, new FloatLiteral(integerLiteral));
                        default:
                            throw new ArgumentOutOfRangeException(nameof(b));
                    }

                case IntegerLiteral integerLiteral:
                    switch (b)
                    {
                        case CharLiteral charLiteral:
                            return (integerLiteral, new IntegerLiteral(charLiteral));
                        case FloatLiteral floatLiteral:
                            return (new FloatLiteral(integerLiteral), floatLiteral);
                        case IntegerLiteral integerLiteral1:
                            return (integerLiteral, integerLiteral1);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(b));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator +(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value + charLiteralB.Value);
                case FloatLiteral floatLiteralA when b is FloatLiteral floatLiteralB:
                    return new FloatLiteral(floatLiteralA.Value + floatLiteralB.Value);
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA + integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator -(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value - charLiteralB.Value);
                case FloatLiteral floatLiteralA when b is FloatLiteral floatLiteralB:
                    return new FloatLiteral(floatLiteralA.Value - floatLiteralB.Value);
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA - integerLiteralB;
                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator *(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value * charLiteralB.Value);
                case FloatLiteral floatLiteralA when b is FloatLiteral floatLiteralB:
                    return new FloatLiteral(floatLiteralA.Value * floatLiteralB.Value);
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA * integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator /(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value / charLiteralB.Value);
                case FloatLiteral floatLiteralA when b is FloatLiteral floatLiteralB:
                    return new FloatLiteral(floatLiteralA.Value / floatLiteralB.Value);
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA / integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator %(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value % charLiteralB.Value);
                case FloatLiteral floatLiteralA when b is FloatLiteral floatLiteralB:
                    return new FloatLiteral(floatLiteralA.Value % floatLiteralB.Value);
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA % integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator &(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value & charLiteralB.Value);
                case FloatLiteral _ when b is FloatLiteral:
                    return null;
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA & integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator |(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value | charLiteralB.Value);
                case FloatLiteral _ when b is FloatLiteral:
                    return null;
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA | integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator ^(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value ^ charLiteralB.Value);
                case FloatLiteral _ when b is FloatLiteral:
                    return null;
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return integerLiteralA ^ integerLiteralB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase BitLeftShift(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value << charLiteralB.Value);
                case FloatLiteral _ when b is FloatLiteral:
                    return null;
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return IntegerLiteral.BitLeftShift(integerLiteralA, integerLiteralB);

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase BitRightShift(OrderedLiteralBase a, OrderedLiteralBase b)
        {
            (a, b) = LiftToCommonType(a, b);
            if (a.GetType() != b.GetType()) throw new ApplicationException($"{a.GetType()} != {b.GetType()}");
            switch (a)
            {
                case CharLiteral charLiteralA when b is CharLiteral charLiteralB:
                    return IntegerLiteral.From(charLiteralA.Value >> charLiteralB.Value);
                case FloatLiteral _ when b is FloatLiteral:
                    return null;
                case IntegerLiteral integerLiteralA when b is IntegerLiteral integerLiteralB:
                    return IntegerLiteral.BitRightShift(integerLiteralA, integerLiteralB);

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator -(OrderedLiteralBase a)
        {
            switch (a)
            {
                case CharLiteral charLiteralA:
                    return IntegerLiteral.From(-charLiteralA.Value);
                case FloatLiteral floatLiteralA:
                    return new FloatLiteral(-floatLiteralA.Value);
                case IntegerLiteral integerLiteralA:
                    return -integerLiteralA;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase operator ~(OrderedLiteralBase a)
        {
            switch (a)
            {
                case CharLiteral charLiteralA:
                    return IntegerLiteral.From(~charLiteralA.Value);
                case FloatLiteral _:
                    return null;
                case IntegerLiteral integerLiteralA:
                    return ~integerLiteralA;

                default:
                    throw new ArgumentOutOfRangeException(nameof(a));
            }
        }

        public static OrderedLiteralBase Max(OrderedLiteralBase val1, OrderedLiteralBase val2)
        {
            return Comparer<OrderedLiteralBase>.Default.Compare(val1, val2) >= 0 ? val1 : val2;
        }

        public static OrderedLiteralBase Min(OrderedLiteralBase val1, OrderedLiteralBase val2)
        {
            return Comparer<OrderedLiteralBase>.Default.Compare(val1, val2) <= 0 ? val1 : val2;
        }
    }
}