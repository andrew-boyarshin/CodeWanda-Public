using System;
using System.Collections;
using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow
{
    public sealed class ValueInterval : IEquatable<ValueInterval>, IComparable<ValueInterval>, IComparable, IValue
    {
        public static readonly ValueInterval Empty =
            new ValueInterval(IntegerLiteral.Zero, IntegerLiteral.Zero, false, false);

        [NotNull] public OrderedLiteralBase From { get; }
        [NotNull] public OrderedLiteralBase To { get; }
        public bool FromInclusive { get; }
        public bool ToInclusive { get; }

        public ValueInterval([NotNull] OrderedLiteralBase from, [NotNull] OrderedLiteralBase to,
            bool fromInclusive = true, bool toInclusive = true)
        {
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
            FromInclusive = fromInclusive;
            ToInclusive = toInclusive;

            if (Comparer<OrderedLiteralBase>.Default.Compare(from, to) > 0)
                throw new ArgumentException(nameof(from));
        }

        public static ValueInterval Single(OrderedLiteralBase value)
        {
            return new ValueInterval(value, value);
        }

        public bool Equals(ValueInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (IsEmpty && other.IsEmpty)
                return true;

            return From.Equals(other.From) && To.Equals(other.To)
                                           && FromInclusive.Equals(other.FromInclusive)
                                           && ToInclusive.Equals(other.ToInclusive);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ValueInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, FromInclusive, ToInclusive);
        }

        public bool IsEmpty
        {
            get
            {
                if (ReferenceEquals(this, Empty)) return true;
                return From.Equals(To)
                       && (!FromInclusive || !ToInclusive);
            }
        }

        public static bool operator ==(ValueInterval left, ValueInterval right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueInterval left, ValueInterval right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(ValueInterval other)
        {
            if (ReferenceEquals(null, other)) return 1;
            if (ReferenceEquals(this, other)) return 0;

            if (IsEmpty && other.IsEmpty)
                return 0;

            var left = Comparer<OrderedLiteralBase>.Default.Compare(From, other.From);
            if (left == 0)
                left = FromInclusive && !other.FromInclusive
                    ? -1 // this.From < other.From
                    : !FromInclusive && other.ToInclusive
                        ? 1 // this.From > other.From
                        : 0; // this.From == other.From
            return left;
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is ValueInterval other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(ValueInterval)}");
        }

        public static bool operator <(ValueInterval left, ValueInterval right)
        {
            return Comparer<ValueInterval>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(ValueInterval left, ValueInterval right)
        {
            return Comparer<ValueInterval>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(ValueInterval left, ValueInterval right)
        {
            return Comparer<ValueInterval>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(ValueInterval left, ValueInterval right)
        {
            return Comparer<ValueInterval>.Default.Compare(left, right) >= 0;
        }

        public static ValueInterval operator &(ValueInterval a, ValueInterval b)
        {
            if (a == null || b == null)
            {
                Log.Warning("ValueInterval: {A} & {B}", a, b);
                return null;
            }

            if (a.IsEmpty || b.IsEmpty)
                return Empty;

            var (orderedFrom, fromInclusive) = Comparer<OrderedLiteralBase>.Default.Compare(a.From, b.From) switch
                {
                {} v when v < 0 => (b.From, b.FromInclusive),
                0 => (a.From, a.FromInclusive && b.FromInclusive),
                {} v when v > 0 => (a.From, a.FromInclusive),
                };

            var (orderedTo, toInclusive) = Comparer<OrderedLiteralBase>.Default.Compare(a.To, b.To) switch
                {
                {} v when v < 0 => (a.To, a.ToInclusive),
                0 => (a.To, a.ToInclusive && b.ToInclusive),
                {} v when v > 0 => (b.To, b.ToInclusive),
                };

            return Comparer<OrderedLiteralBase>.Default.Compare(orderedFrom, orderedTo) > 0
                ? Empty
                : new ValueInterval(orderedFrom, orderedTo, fromInclusive, toInclusive);
        }

        public static ValueInterval operator |(ValueInterval a, ValueInterval b)
        {
            if (a == null || b == null)
            {
                Log.Warning("ValueInterval: {A} | {B}", a, b);
                return null;
            }

            if (!a.IsEmpty && b.IsEmpty)
                return a;
            if (a.IsEmpty && !b.IsEmpty)
                return b;

            var (orderedFrom, fromInclusive) = Comparer<OrderedLiteralBase>.Default.Compare(a.From, b.From) switch
                {
                {} v when v < 0 => (a.From, a.FromInclusive),
                0 => (a.From, a.FromInclusive || b.FromInclusive),
                {} v when v > 0 => (b.From, b.FromInclusive),
                };

            var (orderedTo, toInclusive) = Comparer<OrderedLiteralBase>.Default.Compare(a.To, b.To) switch
                {
                {} v when v < 0 => (b.To, b.ToInclusive),
                0 => (a.To, a.ToInclusive || b.ToInclusive),
                {} v when v > 0 => (a.To, a.ToInclusive),
                };

            return Comparer<OrderedLiteralBase>.Default.Compare(orderedFrom, orderedTo) > 0
                ? Empty
                : new ValueInterval(orderedFrom, orderedTo, fromInclusive, toInclusive);
        }

        public override string ToString()
        {
            if (IsEmpty)
                return "<empty>";

            if (Equals(From, To) || From.ToString() == To.ToString())
                return $"{From}";

            return $"{From}..{To}";
        }

        public IEnumerable<T> Iterate<T>(T increment) where T : OrderedLiteralBase
        {
            return new IntervalIterator<T>(this, increment);
        }

        private class IntervalIterator<T> : IEnumerable<T>, IEnumerator<T> where T : OrderedLiteralBase
        {
            public IntervalIterator([NotNull] ValueInterval instance, [NotNull] T increment)
            {
                Instance = instance;
                Increment = increment;
                Current = default;
                From = (T) instance.From;
                To = (T) instance.To;
            }

            [NotNull] private ValueInterval Instance { get; }
            [NotNull] private T From { get; }
            [NotNull] private T To { get; }
            [NotNull] private T Increment { get; }
            private bool Init { get; set; }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool MoveNext()
            {
                if (!Init)
                {
                    Current = From;
                    Init = true;
                    if (!Instance.FromInclusive)
                    {
                        Current = (T) (Current + Increment);
                    }
                }
                else
                {
                    Current = (T) (Current + Increment);
                }

                var compare = Comparer<T>.Default.Compare(Current, To);
                return Instance.ToInclusive ? compare <= 0 : compare < 0;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}