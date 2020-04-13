using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Logic;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public static class ValueUtilities
    {
        [CanBeNull]
        public static ValueInterval ToInterval([NotNull] AnalysisContext context,
            [NotNull] LogicExpressionBase expression,
            [NotNull] ILValue lValue)
        {
            switch (expression)
            {
                case BinaryLogicExpression binaryLogicExpression:
                    if (binaryLogicExpression.Left is OrderedLiteralBase && binaryLogicExpression.Right is Variable)
                    {
                        binaryLogicExpression = binaryLogicExpression.Flip();
                    }

                    var left = binaryLogicExpression.Left;
                    var right = context[binaryLogicExpression.Right];
                    if (right == null)
                        return null;

                    ILValue variant = null;
                    OrderedLiteralBase literal = null;
                    switch (left)
                    {
                        case ILValue valueVariantBase:
                            variant = valueVariantBase;
                            break;
                        case OrderedLiteralBase orderedLiteralBase:
                            literal = orderedLiteralBase;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(left));
                    }

                    switch (right)
                    {
                        case ILValue valueVariantBase:
                            variant = valueVariantBase;
                            break;
                        case OrderedLiteralBase orderedLiteralBase:
                            literal = orderedLiteralBase;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(right));
                    }

                    if (variant == null || literal == null)
                    {
                        return null;
                    }

                    if (Equals(lValue, variant) ||
                        variant is IVariantWithStorageVariable variantWithStorage &&
                        Equals(variantWithStorage.StorageVariable, lValue))
                    {
                        return OperatorToInterval(binaryLogicExpression.Operator,
                            literal,
                            variant.TypeRef);
                    }
                    else
                    {
                        return null;
                    }

                case UnaryLogicExpression unaryLogicExpression:
                    var value = context[unaryLogicExpression.Value];
                    if (value == null)
                        return null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }

            throw new NotImplementedException();
        }

        [CanBeNull]
        private static ValueInterval OperatorToInterval(LogicBase op, OrderedLiteralBase value, TypeReference valueType)
        {
            switch (op)
            {
                case AndLogic _:
                    goto default;
                case EqualLogic _:
                    return ValueInterval.Single(value);
                case GreaterLogic _:
                    return new ValueInterval(value, valueType.MaxValue, fromInclusive: false);
                case GreaterOrEqualLogic _:
                    return new ValueInterval(value, valueType.MaxValue);
                case LessLogic _:
                    return new ValueInterval(valueType.MinValue, value, toInclusive: false);
                case LessOrEqualLogic _:
                    return new ValueInterval(valueType.MinValue, value);
                case NotEqualLogic _:
                    return null;
                case OrLogic _:
                    goto default;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static readonly Func<ValueInterval, ValueInterval, ValueInterval> IterateValuesAndFunc = (a, b) => a & b;
        public static readonly Func<ValueInterval, ValueInterval, ValueInterval> IterateValuesOrFunc = (a, b) => a | b;

        public static IDataValue IterateValues([NotNull] IEnumerable<IDataValue> dataValues,
            [NotNull] Func<ValueInterval, ValueInterval, ValueInterval> func)
        {
            var values = dataValues as IDataValue[] ?? dataValues.ToArray();
            return IterateValues(values[1..], values[0], values.Length, func);
        }

        public static IDataValue IterateValues([NotNull] IEnumerable<IDataValue> dataValues, IDataValue current,
            int count,
            [NotNull] Func<ValueInterval, ValueInterval, ValueInterval> func)
        {
            var values = dataValues as IDataValue[] ?? dataValues.ToArray();
            foreach (var value in values)
            {
                switch (value)
                {
	                case IDataValueInterval dataValueInterval when current is IDataValueInterval interval:
	                {
		                var valueInterval = func(interval.Interval, dataValueInterval.Interval);
		                valueInterval ??= ValueInterval.Empty;

		                var constraints = current.Constraints.Concat(dataValueInterval.Constraints).ToArray();
		                current = new DataValueInterval(valueInterval, constraints);

		                break;
	                }
                
                    case IDataValueArray _:
                    case IDataValueLiteral _:
                    case IDataValueLValueRef _:
                        if (count > 1)
                            Log.Debug("Ignoring all changes: {Values}", values);
                        return current;

                    case IDataValueUnknown _:
                        Log.Verbose("Returning Unknown value");
                        return value;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            return current;
        }
    }
}