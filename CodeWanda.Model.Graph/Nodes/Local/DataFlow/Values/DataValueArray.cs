using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public sealed class DataValueArray : IDataValueArray, IEquatable<DataValueArray>
    {
        public DataValueArray([NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> constraints)
        {
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public IReadOnlyList<LogicExpressionBase> Constraints { get; }

        public ValueInterval Solve(ILValue lValue, AnalysisContext context)
        {
            return null;
        }

        public IDataValue WithConstraints(IReadOnlyList<LogicExpressionBase> constraints)
        {
            return new DataValueArray(constraints);
        }

        public override string ToString()
        {
            return "{}";
        }

        public bool Equals(DataValueArray other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataValueArray other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Constraints.GetHashCode();
        }

        public static bool operator ==(DataValueArray left, DataValueArray right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataValueArray left, DataValueArray right)
        {
            return !Equals(left, right);
        }
    }
}