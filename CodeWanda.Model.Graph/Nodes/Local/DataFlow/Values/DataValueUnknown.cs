using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public sealed class DataValueUnknown : IDataValueUnknown, IEquatable<DataValueUnknown>
    {
        public override string ToString() => "?";

        public IReadOnlyList<LogicExpressionBase> Constraints { get; } = Array.Empty<LogicExpressionBase>();

        public ValueInterval Solve(ILValue lValue, AnalysisContext context)
        {
            return null;
        }

        public IDataValue WithConstraints(IReadOnlyList<LogicExpressionBase> constraints)
        {
            return new DataValueUnknown();
        }

        public bool Equals(DataValueUnknown other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataValueUnknown other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Constraints.GetHashCode();
        }

        public static bool operator ==(DataValueUnknown left, DataValueUnknown right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataValueUnknown left, DataValueUnknown right)
        {
            return !Equals(left, right);
        }
    }
}