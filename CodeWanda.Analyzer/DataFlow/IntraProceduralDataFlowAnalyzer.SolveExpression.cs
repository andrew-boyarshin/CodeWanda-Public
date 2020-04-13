using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Logic;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using JetBrains.Annotations;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private bool SolveExpression(
            [NotNull] AnalysisContext context,
            [NotNull] LogicExpressionBase logicExpressionBase,
            List<(ValueVariantBase, IDataValue)> trueChanges,
            List<(ValueVariantBase, IDataValue)> falseChanges)
        {
            IValue left = null, right = null, unary = null;

            repeat:
            switch (logicExpressionBase)
            {
                case BinaryLogicExpression binaryLogicExpression:
                    left = context[binaryLogicExpression.Left];
                    right = context[binaryLogicExpression.Right];

                    if (left == null)
                        return false;
                    if (right == null)
                        return false;

                    switch (left is LiteralBase, right is LiteralBase)
                    {
                        case (true, false):
                            logicExpressionBase = binaryLogicExpression.Flip();
                            goto repeat;
                    }

                    break;
                case UnaryLogicExpression unaryLogicExpression:
                    unary = context[unaryLogicExpression.Value];
                    if (unary == null)
                        return false;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logicExpressionBase));
            }

            switch (logicExpressionBase.Operator)
            {
                case AndLogic _ when logicExpressionBase is BinaryLogicExpression:
                    switch (left)
                    {
                        case LogicExpressionBase logicExpression:
                            if (!SolveExpression(context, logicExpression, trueChanges, null))
                                return false;

                            break;
                        default:
                            return false;
                    }

                    switch (right)
                    {
                        case LogicExpressionBase logicExpression:
                            if (!SolveExpression(context, logicExpression, trueChanges, null))
                                return false;

                            break;
                        default:
                            return false;
                    }

                    return true;
                case OrLogic _ when logicExpressionBase is BinaryLogicExpression:
                    switch (left)
                    {
                        case LogicExpressionBase logicExpression:
                            if (!SolveExpression(context, logicExpression.Negate(), falseChanges, null))
                                return false;

                            break;
                        default:
                            return false;
                    }

                    switch (right)
                    {
                        case LogicExpressionBase logicExpression:
                            if (!SolveExpression(context, logicExpression.Negate(), falseChanges, null))
                                return false;

                            break;
                        default:
                            return false;
                    }

                    return true;
                case AndLogic _:
                case OrLogic _:
                    throw new ArgumentOutOfRangeException(nameof(logicExpressionBase));
            }

            if (left is ValueVariantBase leftVariant)
            {
                switch (logicExpressionBase.Operator)
                {
	                case EqualLogic _:
                    {
                        if (trueChanges != null)
                        {
                            var variantValue = ConditionAssign(right, context, leftVariant.Value);

                            var dataValue = variantValue ?? new DataValueUnknown();

                            trueChanges.Add((leftVariant, dataValue));
                        }

                        return true;
                    }

                    case GreaterLogic _:
                    case GreaterOrEqualLogic _:
                    case LessLogic _:
                    case LessOrEqualLogic _:
                    {
                        if (trueChanges != null)
                        {
                            var constraints = leftVariant.Value.Constraints
                                .Concat(new[] {logicExpressionBase});

                            var dataValue = leftVariant.Value.WithConstraints(constraints.ToArray());

                            trueChanges.Add((leftVariant, dataValue));
                        }

                        if (falseChanges != null)
                        {
                            var constraints = leftVariant.Value.Constraints
                                .Concat(new[] {logicExpressionBase.Negate()});

                            var dataValue = leftVariant.Value.WithConstraints(constraints.ToArray());

                            falseChanges.Add((leftVariant, dataValue));
                        }

                        return true;
                    }

                    case NotEqualLogic _:
                    {
                        if (falseChanges != null)
                        {
                            var variantValue = ConditionAssign(right, context, leftVariant.Value);

                            var dataValue = variantValue ?? new DataValueUnknown();

                            falseChanges.Add((leftVariant, dataValue));
                        }

                        return true;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }
}