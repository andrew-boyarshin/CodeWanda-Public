using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public class CharLiteral : OrderedLiteralBase, IComparable<CharLiteral>, IComparable, IEquatable<CharLiteral>
    {
        public CharLiteral(char value)
        {
            Value = value;
        }

        public CharLiteral([NotNull] OrderedLiteralBase other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            switch (other)
            {
                case CharLiteral charLiteral:
                    Value = charLiteral.Value;
                    break;
                case FloatLiteral floatLiteral:
                    Value = (char) floatLiteral.Value;
                    break;
                case IntegerLiteral integerLiteral:
                    Value = checked((char) ((long) integerLiteral.AbsoluteValue * integerLiteral.Sign));

                    if (integerLiteral.Sign == 0 && integerLiteral.AbsoluteValue != 0)
                    {
                        throw new ArgumentException(nameof(integerLiteral));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public char Value { get; }

        public override string ToString()
        {
            return $"'{Value}'";
        }

        public override int CompareTo(OrderedLiteralBase other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            switch (other)
            {
                case CharLiteral charLiteral:
                    return CompareTo(charLiteral);
                case FloatLiteral floatLiteral:
                    return CompareTo(IntegerLiteral.From((long) floatLiteral.Value));
                case IntegerLiteral integerLiteral:
                    return CompareTo(new CharLiteral(integerLiteral));
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
                    return Equals(charLiteral);
                case FloatLiteral floatLiteral:
                    return Equals(IntegerLiteral.From((long) floatLiteral.Value));
                case IntegerLiteral integerLiteral:
                    return Equals(new CharLiteral(integerLiteral));
                default:
                    throw new ArgumentOutOfRangeException(nameof(other));
            }
        }

        public bool Equals(CharLiteral other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
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

        public static bool operator ==(CharLiteral left, CharLiteral right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CharLiteral left, CharLiteral right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(CharLiteral other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is CharLiteral other
                ? CompareTo(other)
                : obj is OrderedLiteralBase orderedLiteralBase
                    ? CompareTo(orderedLiteralBase)
                    : throw new ArgumentException($"Object must be of type {nameof(CharLiteral)}");
        }

        public static bool operator <(CharLiteral left, CharLiteral right)
        {
            return Comparer<CharLiteral>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(CharLiteral left, CharLiteral right)
        {
            return Comparer<CharLiteral>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(CharLiteral left, CharLiteral right)
        {
            return Comparer<CharLiteral>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(CharLiteral left, CharLiteral right)
        {
            return Comparer<CharLiteral>.Default.Compare(left, right) >= 0;
        }
    }
}