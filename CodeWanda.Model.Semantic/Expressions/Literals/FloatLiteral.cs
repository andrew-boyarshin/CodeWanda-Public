using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public class FloatLiteral : OrderedLiteralBase, IComparable<FloatLiteral>, IComparable, IEquatable<FloatLiteral>
    {
        public FloatLiteral()
        {
        }

        public FloatLiteral([NotNull] OrderedLiteralBase other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            switch (other)
            {
                case CharLiteral charLiteral:
                    Value = charLiteral.Value;
                    break;
                case FloatLiteral floatLiteral:
                    Value = floatLiteral.Value;
                    break;
                case IntegerLiteral integerLiteral:
                    Value = integerLiteral.AbsoluteValue;
                    Value *= integerLiteral.Sign;

                    if (integerLiteral.Sign == 0 && integerLiteral.AbsoluteValue != 0)
                    {
                        throw new ArgumentException(nameof(integerLiteral));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public FloatLiteral(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public override string ToString()
        {
            return $"{Value}";
        }

        public override int CompareTo(OrderedLiteralBase other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            switch (other)
            {
                case CharLiteral charLiteral:
                    return CompareTo(new FloatLiteral(IntegerLiteral.From(charLiteral.Value)));
                case FloatLiteral floatLiteral:
                    return CompareTo(floatLiteral);
                case IntegerLiteral integerLiteral:
                    return CompareTo(new FloatLiteral(integerLiteral));
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public override bool Equals(OrderedLiteralBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            switch (other)
            {
                case CharLiteral charLiteral:
                    return Equals(IntegerLiteral.From(charLiteral.Value));
                case FloatLiteral floatLiteral:
                    return Equals(floatLiteral);
                case IntegerLiteral integerLiteral:
                    return Equals(new FloatLiteral(integerLiteral));
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public int CompareTo(FloatLiteral other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is FloatLiteral other
                ? CompareTo(other)
                : obj is OrderedLiteralBase orderedLiteralBase
                    ? CompareTo(orderedLiteralBase)
                    : throw new ArgumentException($"Object must be of type {nameof(FloatLiteral)}");
        }

        public bool Equals(FloatLiteral other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Math.Abs(Value - other.Value) < double.Epsilon;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OrderedLiteralBase other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(FloatLiteral left, FloatLiteral right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FloatLiteral left, FloatLiteral right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(FloatLiteral left, FloatLiteral right)
        {
            return Comparer<FloatLiteral>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(FloatLiteral left, FloatLiteral right)
        {
            return Comparer<FloatLiteral>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(FloatLiteral left, FloatLiteral right)
        {
            return Comparer<FloatLiteral>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(FloatLiteral left, FloatLiteral right)
        {
            return Comparer<FloatLiteral>.Default.Compare(left, right) >= 0;
        }
    }
}