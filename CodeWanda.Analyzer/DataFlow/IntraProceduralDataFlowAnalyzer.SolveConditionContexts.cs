using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private void SolveConditionContexts(
            [NotNull] GraphDataFlowConditionalNode node,
            [NotNull] LogicExpressionBase expression,
            [NotNull] GraphNode[] path)
        {
            var trueChanges = new List<(ValueVariantBase, IDataValue)>();
            var falseChanges = new List<(ValueVariantBase, IDataValue)>();

            if (!SolveExpression(node.EnterAnalysisContext, expression, trueChanges, falseChanges))
            {
                Log.Information("Logic: {Operator} -> {Expression}",
                    expression.Operator,
                    expression);
            }

            foreach (var grouping in trueChanges.GroupBy(x => x.Item1, x => x.Item2))
            {
                var (variant, dataValue) = (grouping.Key, grouping.ToArray());
                var newVariant = variant.Clone(node.EnterAnalysisContext, path);
                newVariant.Value = ValueUtilities.IterateValues(dataValue, variant.Value, dataValue.Length + 1,
                    ValueUtilities.IterateValuesAndFunc);
                node.TrueExitAnalysisContext[variant] = newVariant;
            }

            foreach (var grouping in falseChanges.GroupBy(x => x.Item1, x => x.Item2))
            {
                var (variant, dataValue) = (grouping.Key, grouping.ToArray());
                var newVariant = variant.Clone(node.EnterAnalysisContext, path);
                newVariant.Value = ValueUtilities.IterateValues(dataValue, variant.Value, dataValue.Length + 1,
                    ValueUtilities.IterateValuesAndFunc);
                node.FalseExitAnalysisContext[variant] = newVariant;
            }
        }
    }
}