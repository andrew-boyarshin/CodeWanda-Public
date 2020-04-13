using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public sealed class DataValueLiteral : IDataValueLiteral, IEquatable<DataValueLiteral>
    {
        public DataValueLiteral([NotNull] LiteralBase literal,
            [NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> constraints)
        {
            Literal = literal ?? throw new ArgumentNullException(nameof(literal));
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
            if (literal is OrderedLiteralBase)
            {
                Log.Warning("Should have created OrderedLiteralBase for {Value}", literal);
            }
        }

        public LiteralBase Literal { get; }
        public IReadOnlyList<LogicExpressionBase> Constraints { get; }

        public ValueInterval Solve(ILValue lValue, AnalysisContext context)
        {
            return null;
        }

        public IDataValue WithConstraints(IReadOnlyList<LogicExpressionBase> constraints)
        {
            return new DataValueLiteral(Literal, constraints);
        }

        public override string ToString() => $"{Literal}";

        public bool Equals(DataValueLiteral other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Literal.Equals(other.Literal) && Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is DataValueLiteral other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Literal, Constraints);
        }

        public static bool operator ==(DataValueLiteral left, DataValueLiteral right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataValueLiteral left, DataValueLiteral right)
        {
            return !Equals(left, right);
        }
    }
}