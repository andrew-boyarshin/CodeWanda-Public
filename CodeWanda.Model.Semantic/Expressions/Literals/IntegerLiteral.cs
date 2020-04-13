using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public class IntegerLiteral : OrderedLiteralBase, IComparable<IntegerLiteral>, IComparable,
        IEquatable<IntegerLiteral>
    {
        public static readonly IntegerLiteral Zero = new IntegerLiteral(0, 0);
        public static readonly IntegerLiteral One = new IntegerLiteral(1, 1);
        public static readonly IntegerLiteral MaxULong = new IntegerLiteral(ulong.MaxValue, 1);
        public static readonly IntegerLiteral MaxUInt = new IntegerLiteral(uint.MaxValue, 1);
        public static readonly IntegerLiteral MaxUShort = new IntegerLiteral(ushort.MaxValue, 1);
        public static readonly IntegerLiteral MaxByte = new IntegerLiteral(byte.MaxValue, 1);
        public static readonly IntegerLiteral MaxLong = new IntegerLiteral(long.MaxValue, 1);
        public static readonly IntegerLiteral MaxInt = new IntegerLiteral(int.MaxValue, 1);
        public static readonly IntegerLiteral MaxShort = new IntegerLiteral((ulong) short.MaxValue, 1);
        public static readonly IntegerLiteral MaxSByte = new IntegerLiteral((ulong) sbyte.MaxValue, 1);
        public static readonly IntegerLiteral MinLong = new IntegerLiteral(0x8000000000000000, -1);
        public static readonly IntegerLiteral MinInt = new IntegerLiteral(0x80000000, -1);
        public static readonly IntegerLiteral MinShort = new IntegerLiteral(0x8000, -1);
        public static readonly IntegerLiteral MinSByte = new IntegerLiteral(0x80, -1);

        public IntegerLiteral()
        {
        }

        public IntegerLiteral([NotNull] OrderedLiteralBase other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            switch (other)
            {
                case CharLiteral charLiteral:
                    AbsoluteValue = (ulong) Math.Abs(charLiteral.Value);
                    Sign = Math.Sign(charLiteral.Value);
                    break;
                case FloatLiteral floatLiteral:
                    AbsoluteValue = (ulong) Math.Abs(floatLiteral.Value);
                    Sign = Math.Sign(floatLiteral.Value);
                    break;
                case IntegerLiteral integerLiteral:
                    AbsoluteValue = integerLiteral.AbsoluteValue;
                    Sign = integerLiteral.Sign;

                    if (integerLiteral.Sign == 0 && integerLiteral.AbsoluteValue != 0)
                    {
                        throw new ArgumentException(nameof(integerLiteral));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public IntegerLiteral(ulong absoluteValue, int sign)
        {
            AbsoluteValue = absoluteValue;
            Sign = sign;
        }

        public ulong AbsoluteValue { get; }
        public int Sign { get; }

        public override string ToString()
        {
            return Sign < 0 ? $"-{AbsoluteValue}" : AbsoluteValue.ToString();
        }

        public long ToLong() => checked((long) AbsoluteValue * Sign);

        public override int CompareTo(OrderedLiteralBase other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            switch (other)
            {
                case CharLiteral charLiteral:
                    return CompareTo(From(charLiteral.Value));
                case FloatLiteral floatLiteral:
                    return CompareTo(From((long) floatLiteral.Value));
                case IntegerLiteral integerLiteral:
                    return CompareTo(integerLiteral);
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
                    return Equals(From(charLiteral.Value));
                case FloatLiteral floatLiteral:
                    return Equals(From((long) floatLiteral.Value));
                case IntegerLiteral integerLiteral:
                    return Equals(integerLiteral);
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public static IntegerLiteral From(ulong value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
                case 0u:
                    return Zero;
                case 1u:
                    return One;
                case byte.MaxValue:
                    return MaxByte;
                case ushort.MaxValue:
                    return MaxUShort;
                case uint.MaxValue:
                    return MaxUInt;
                case ulong.MaxValue:
                    return MaxULong;
            }

            return new IntegerLiteral(value, value > 0 ? 1 : 0);
        }

        public static IntegerLiteral From(long value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
                case 0:
                    return Zero;
                case 1:
                    return One;
                case sbyte.MinValue:
                    return MinSByte;
                case short.MinValue:
                    return MinShort;
                case int.MinValue:
                    return MinInt;
                case long.MinValue:
                    return MinLong;
                case sbyte.MaxValue:
                    return MaxSByte;
                case short.MaxValue:
                    return MaxShort;
                case int.MaxValue:
                    return MaxInt;
                case long.MaxValue:
                    return MaxLong;
            }

            var absoluteValue = value < 0 ? (ulong) -value : (ulong) value;
            var from = new IntegerLiteral(absoluteValue, Math.Sign(value));

            return from;
        }

        public int CompareTo(IntegerLiteral other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var signComparison = Sign.CompareTo(other.Sign);
            if (signComparison != 0) return signComparison;
            if (Sign == -1)
                return -AbsoluteValue.CompareTo(other.AbsoluteValue);
            return AbsoluteValue.CompareTo(other.AbsoluteValue);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is IntegerLiteral other
                ? CompareTo(other)
                : obj is OrderedLiteralBase orderedLiteralBase
                    ? CompareTo(orderedLiteralBase)
                    : throw new ArgumentException($"Object must be of type {nameof(IntegerLiteral)}");
        }

        public bool Equals(IntegerLiteral other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AbsoluteValue == other.AbsoluteValue && Sign == other.Sign;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is OrderedLiteralBase other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AbsoluteValue, Sign);
        }

        public static bool operator ==(IntegerLiteral left, IntegerLiteral right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntegerLiteral left, IntegerLiteral right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(IntegerLiteral left, IntegerLiteral right)
        {
            return Comparer<IntegerLiteral>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(IntegerLiteral left, IntegerLiteral right)
        {
            return Comparer<IntegerLiteral>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(IntegerLiteral left, IntegerLiteral right)
        {
            return Comparer<IntegerLiteral>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(IntegerLiteral left, IntegerLiteral right)
        {
            return Comparer<IntegerLiteral>.Default.Compare(left, right) >= 0;
        }

        public static IntegerLiteral operator +(IntegerLiteral a, IntegerLiteral b)
        {
            var absoluteValue = a.AbsoluteValue;
            var sign = a.Sign;

            if (absoluteValue == 0)
            {
                absoluteValue = b.AbsoluteValue;
                sign = b.Sign;
            }
            else if (b.Sign == sign)
            {
                absoluteValue += b.AbsoluteValue;
            }
            else if (b.AbsoluteValue < absoluteValue)
            {
                absoluteValue -= b.AbsoluteValue;
            }
            else if (b.AbsoluteValue == absoluteValue)
            {
                absoluteValue = 0;
                sign = 0;
            }
            else
            {
                absoluteValue = b.AbsoluteValue - absoluteValue;
                sign = -sign;
            }

            return new IntegerLiteral(absoluteValue, sign);
        }

        public static IntegerLiteral operator -(IntegerLiteral a, IntegerLiteral b)
        {
            return a + new IntegerLiteral(-b);
        }

        public static IntegerLiteral operator *(IntegerLiteral a, IntegerLiteral b)
        {
            var absoluteValue = a.AbsoluteValue;
            var sign = a.Sign;

            absoluteValue *= b.AbsoluteValue;
            sign *= b.Sign;

            return new IntegerLiteral(absoluteValue, sign);
        }

        public static IntegerLiteral operator /(IntegerLiteral a, IntegerLiteral b)
        {
            var absoluteValue = a.AbsoluteValue;
            var sign = a.Sign;

            absoluteValue /= b.AbsoluteValue;
            sign *= b.Sign;

            return new IntegerLiteral(absoluteValue, sign);
        }

        public static IntegerLiteral operator %(IntegerLiteral a, IntegerLiteral b)
        {
            return a - a / b * b;
        }

        public static IntegerLiteral operator &(IntegerLiteral a, IntegerLiteral b)
        {
            return From(a.ToLong() & b.ToLong());
        }

        public static IntegerLiteral operator |(IntegerLiteral a, IntegerLiteral b)
        {
            return From(a.ToLong() | b.ToLong());
        }

        public static IntegerLiteral operator ^(IntegerLiteral a, IntegerLiteral b)
        {
            return From(a.ToLong() ^ b.ToLong());
        }

        public static IntegerLiteral BitLeftShift(IntegerLiteral a, IntegerLiteral b)
        {
            return From(a.ToLong() << checked((int) b.ToLong()));
        }

        public static IntegerLiteral BitRightShift(IntegerLiteral a, IntegerLiteral b)
        {
            return From(a.ToLong() >> checked((int) b.ToLong()));
        }

        public static IntegerLiteral operator -(IntegerLiteral a)
        {
            var absoluteValue = a.AbsoluteValue;
            var sign = a.Sign;

            return new IntegerLiteral(absoluteValue, -sign);
        }

        public static IntegerLiteral operator ~(IntegerLiteral a)
        {
            return From(~a.ToLong());
        }
    }
}