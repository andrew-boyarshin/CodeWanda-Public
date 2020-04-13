using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public sealed class DataValueLValueRef : IDataValueLValueRef, IEquatable<DataValueLValueRef>
    {
        public DataValueLValueRef([NotNull] ILValue target,
            [NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> constraints)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public ILValue Target { get; }
        public IReadOnlyList<LogicExpressionBase> Constraints { get; }

        public override string ToString() => $"{Target}";

        public ValueInterval Solve(ILValue lValue, AnalysisContext context)
        {
            var variant = context[Target];
            return variant.Value.Solve(lValue, context);
        }

        public IDataValue WithConstraints(IReadOnlyList<LogicExpressionBase> constraints)
        {
            return new DataValueLValueRef(Target, constraints);
        }

        public bool Equals(DataValueLValueRef other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Target.Equals(other.Target) && Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataValueLValueRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Target, Constraints);
        }

        public static bool operator ==(DataValueLValueRef left, DataValueLValueRef right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataValueLValueRef left, DataValueLValueRef right)
        {
            return !Equals(left, right);
        }
    }
}