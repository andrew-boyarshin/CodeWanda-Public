using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess
{
    public class SubscriptOperator : MemberAccessBase, ILValue, IEquatable<SubscriptOperator>
    {
        public SubscriptOperator([NotNull] ILValue array, [NotNull] IValue index)
        {
            Array = array ?? throw new ArgumentNullException(nameof(array));
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }

        [NotNull] public ILValue Array { get; }
        [NotNull] public IValue Index { get; }

        public override string ToString()
        {
            return $"{Array}[{Index}]";
        }

        public bool Equals(SubscriptOperator other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Array, other.Array) && Equals(Index, other.Index);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscriptOperator) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Array, Index);
        }

        public static bool operator ==(SubscriptOperator left, SubscriptOperator right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SubscriptOperator left, SubscriptOperator right)
        {
            return !Equals(left, right);
        }

        public override TypeReference TypeRef
        {
            get => Array.TypeRef is ArrayTypeRef arrayTypeRef
                ? arrayTypeRef.Inner
                : Array.TypeRef;
            set => throw new ApplicationException();
        }
    }
}