using System;
using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Logic;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Expressions.Operators;
using CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Unary;

namespace CodeWanda.Model.Semantic.Utilities
{
    public static partial class Extensions
    {
        public static IEnumerable<IValue> DescendantValues(this IValue self)
        {
            if (self != null)
                yield return self;

            switch (self)
            {
                case null:
                    break;
                case ParameterVariable _:
                case DummyVariable _:
                case Variable _:
                    break;
                case AssignmentBase assignmentBase:
                    foreach (var descendantExpression in assignmentBase.Left.DescendantValues())
                        yield return descendantExpression;
                    foreach (var descendantExpression in assignmentBase.Right.DescendantValues())
                        yield return descendantExpression;
                    break;
                case BoolLiteral _:
                case CharLiteral _:
                case FloatLiteral _:
                case IntegerLiteral _:
                case NullLiteral _:
                case StringLiteral _:
                case LiteralBase _:
                case AndLogic _:
                case EqualLogic _:
                case GreaterLogic _:
                case GreaterOrEqualLogic _:
                case LessLogic _:
                case LessOrEqualLogic _:
                case NotEqualLogic _:
                case OrLogic _:
                case LogicBase _:
                    break;
                case BinaryLogicExpression binaryLogicExpression:
                    foreach (var descendantExpression in binaryLogicExpression.Left.DescendantValues())
                        yield return descendantExpression;
                    foreach (var descendantExpression in binaryLogicExpression.Operator.DescendantValues())
                        yield return descendantExpression;
                    foreach (var descendantExpression in binaryLogicExpression.Right.DescendantValues())
                        yield return descendantExpression;
                    break;
                case UnaryLogicExpression unaryLogicExpression:
                    foreach (var descendantExpression in unaryLogicExpression.Operator.DescendantValues())
                        yield return descendantExpression;
                    foreach (var descendantExpression in unaryLogicExpression.Value.DescendantValues())
                        yield return descendantExpression;
                    break;
                case LogicExpressionBase _:
                    goto default;
                case SubscriptOperator subscriptOperator:
                    foreach (var descendantValue in subscriptOperator.Array.DescendantValues())
                        yield return descendantValue;
                    foreach (var descendantValue in subscriptOperator.Index.DescendantValues())
                        yield return descendantValue;
                    break;
                case MemberAccessBase _:
                    goto default;
                case FunctionCallOperator functionCallOperator:
                    foreach (var argument in functionCallOperator.Arguments)
                    foreach (var descendantValue in argument.DescendantValues())
                        yield return descendantValue;

                    break;
                case BinaryAddOperator _:
                case BinaryBitAndOperator _:
                case BinaryBitOrOperator _:
                case BinaryBitShiftLeftOperator _:
                case BinaryBitShiftRightOperator _:
                case BinaryBitXorOperator _:
                case BinaryDivideOperator _:
                case BinaryMultiplyOperator _:
                case BinaryRemainderOperator _:
                case BinarySubtractOperator _:
                case BinaryPureBase _:
                case UnaryMinusOperator _:
                case UnaryNegateOperator _:
                case UnaryPureBase _:
                case PureBase _:
                case OperatorBase _:
                    break;
                case FunctionCallParameter functionCallParameter:
                    foreach (var descendantExpression in functionCallParameter.Value.DescendantValues())
                        yield return descendantExpression;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(self));
            }
        }
    }
}