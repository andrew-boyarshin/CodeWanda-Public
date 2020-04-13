using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public sealed class DataValueInterval : IDataValueInterval, IEquatable<DataValueInterval>
    {
        public DataValueInterval([NotNull] ValueInterval interval,
            [NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> constraints)
        {
            Interval = interval ?? throw new ArgumentNullException(nameof(interval));
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public ValueInterval Interval { get; }

        public ValueInterval Solve(ILValue lValue, AnalysisContext context)
        {
            return Constraints
                .Select(next => ValueUtilities.ToInterval(context, next, lValue))
                .Where(x => x != null)
                .Aggregate(Interval, (current, next) => current & next);
        }

        public IDataValue WithConstraints(IReadOnlyList<LogicExpressionBase> constraints)
        {
            return new DataValueInterval(Interval, constraints);
        }

        public IReadOnlyList<LogicExpressionBase> Constraints { get; }

        public override string ToString() => $"{Interval}";

        public bool Equals(DataValueInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Interval.Equals(other.Interval) && Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataValueInterval other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Interval, Constraints);
        }

        public static bool operator ==(DataValueInterval left, DataValueInterval right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataValueInterval left, DataValueInterval right)
        {
            return !Equals(left, right);
        }
    }
}