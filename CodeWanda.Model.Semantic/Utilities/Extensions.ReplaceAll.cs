using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Logic;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Expressions.Operators;
using CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Utilities
{
    public static partial class Extensions
    {
        [Pure]
        public static IValue ReplaceAll(this IValue self, IValue what, IValue with)
        {
            if (Equals(self, what))
                return with;

            switch (self)
            {
                case null:
                    return null;
                case Variable variable:
                    return variable;
                case OperatorAssignment operatorAssignment:
                    return new OperatorAssignment(
                        (ILValue) operatorAssignment.Left.ReplaceAll(what, with),
                        operatorAssignment.Right.ReplaceAll(what, with),
                        operatorAssignment.Operator);
                case SimpleAssignment simpleAssignment:
                    return new SimpleAssignment(
                        (ILValue) simpleAssignment.Left.ReplaceAll(what, with),
                        simpleAssignment.Right.ReplaceAll(what, with));
                case AssignmentBase _:
                    goto default;
                case LiteralBase literalBase:
                    return literalBase;
                case LogicBase logicBase:
                    return logicBase;
                case BinaryLogicExpression binaryLogicExpression:
                    return new BinaryLogicExpression(
                        (LogicBase) binaryLogicExpression.Operator.ReplaceAll(what, with),
                        binaryLogicExpression.Left.ReplaceAll(what, with),
                        binaryLogicExpression.Right.ReplaceAll(what, with));
                case UnaryLogicExpression unaryLogicExpression:
                    return new UnaryLogicExpression(
                        (LogicBase) unaryLogicExpression.Operator.ReplaceAll(what, with),
                        unaryLogicExpression.Value.ReplaceAll(what, with));
                case LogicExpressionBase _:
                    goto default;
                case SubscriptOperator subscriptOperator:
                    return new SubscriptOperator(
                        (ILValue) subscriptOperator.Array.ReplaceAll(what, with),
                        subscriptOperator.Index.ReplaceAll(what, with));
                case MemberAccessBase _:
                    goto default;
                case FunctionCallOperator functionCallOperator:
                    for (var index = 0; index < functionCallOperator.Arguments.Count; index++)
                    {
                        var argument = functionCallOperator.Arguments[index];
                        functionCallOperator.Arguments[index] = (FunctionCallParameter) argument.ReplaceAll(what, with);
                    }

                    return functionCallOperator;
                case OperatorBase operatorBase:
                    return operatorBase;
                case FunctionCallParameter functionCallParameter:
                    return new FunctionCallParameter
                    {
                        Value = functionCallParameter.Value.ReplaceAll(what, with),
                        Out = functionCallParameter.Out,
                        Ref = functionCallParameter.Ref
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(self));
            }
        }
    }
}