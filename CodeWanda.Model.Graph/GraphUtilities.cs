using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph
{
    public static class GraphUtilities
    {
        private static IEnumerable<GraphNode> IterateGraphNodesRecursiveInternal(this GraphNode node,
            ISet<GraphNode> set)
        {
            foreach (var graphNode in node.AllOutgoing)
            {
                if (!set.Add(graphNode))
                    continue;

                yield return graphNode;
                foreach (var child in IterateGraphNodesRecursiveInternal(graphNode, set))
                    yield return child;
            }
        }

        public static IEnumerable<GraphNode> IterateGraphNodesRecursive([DisallowNull] this GraphNode node,
	        bool includeStartNode)
        {
	        if (node == null) throw new ArgumentNullException(nameof(node));

	        var set = new HashSet<GraphNode> { node };

	        if (includeStartNode)
		        yield return node;

            foreach (var graphNode in IterateGraphNodesRecursiveInternal(node, set))
                yield return graphNode;
        }

        [CanBeNull]
        public static ValueInterval ApplyPossibleVariants(this BinaryPureBase self,
            IValue value,
            IValue operand)
        {
            var variantsFrom = new List<IValue>();
            var variantsTo = new List<IValue>();
            var rangeValueItems = new List<OrderedLiteralBase>();
            var rangeOperandItems = new List<OrderedLiteralBase>();
            switch (value)
            {
                case ValueVariantBase variableVariant when variableVariant.Value is DataValueInterval interval:
                    rangeValueItems.Add(interval.Interval.From);
                    rangeValueItems.Add(interval.Interval.To);
                    break;
                case OrderedLiteralBase orderedLiteral:
                    rangeValueItems.Add(orderedLiteral);
                    break;
                default:
                    Log.Warning("ApplyPossibleVariants: Unordered {Value}", value);
                    break;
            }

            switch (operand)
            {
                case ValueVariantBase variableVariant when variableVariant.Value is DataValueInterval interval:
                    rangeOperandItems.Add(interval.Interval.From);
                    rangeOperandItems.Add(interval.Interval.To);
                    break;
                case OrderedLiteralBase orderedLiteral:
                    rangeOperandItems.Add(orderedLiteral);
                    break;
                default:
                    Log.Warning("ApplyPossibleVariants: Unordered {Value}", operand);
                    break;
            }

            rangeValueItems = rangeValueItems.Distinct().ToList();
            rangeOperandItems = rangeOperandItems.Distinct().ToList();

            foreach (var (rangeValueItem, rangeOperandItem) in from rangeValueItem in rangeValueItems
                from rangeOperandItem in rangeOperandItems
                select (rangeValueItem, rangeOperandItem))
            {
                if (rangeValueItem == null)
                    continue;
                if (rangeOperandItem == null)
                    continue;
                try
                {
                    var (from, to) = self.Apply(rangeValueItem, rangeOperandItem);
                    variantsFrom.Add(from);
                    variantsTo.Add(to);
                }
                catch (Exception)
                {
                    Log.Warning("ApplyPossibleVariants: {Value} ({ValueType}) {Operator} {Right} ({RightType})",
                        rangeValueItem,
                        rangeValueItem.GetType().Name,
                        self,
                        rangeOperandItem,
                        rangeOperandItem.GetType().Name);
                }
            }

            variantsFrom.Sort();
            variantsTo.Sort();

            var fromMin = variantsFrom.OfType<OrderedLiteralBase>().FirstOrDefault();
            var toMax = variantsTo.OfType<OrderedLiteralBase>().LastOrDefault();
            if (fromMin == null || toMax == null)
                return null;
            return new ValueInterval(fromMin, toMax);
        }
    }
}