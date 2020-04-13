using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public interface IDataValue
    {
        [NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> Constraints { get; }

        [CanBeNull]
        ValueInterval Solve([NotNull] ILValue lValue, [NotNull] AnalysisContext context);

        [NotNull]
        IDataValue WithConstraints([NotNull] [ItemNotNull] IReadOnlyList<LogicExpressionBase> constraints);
    }
}