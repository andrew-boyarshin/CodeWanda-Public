using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.Logic;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess;
using CodeWanda.Model.Semantic.Expressions.Operators.Other;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure.Binary;
using CodeWanda.Model.Semantic.Statements;
using CodeWanda.Model.Semantic.Types;
using CodeWanda.Model.Semantic.Utilities;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static List<ExpressionBase> ConvertRoslynExpressionToWandaExpression(
            SimpleCompoundStatement wandaBlock,
            ExpressionSyntax expressionSyntax)
        {
            var result = new List<ExpressionBase>();
            switch (expressionSyntax)
            {
                case AssignmentExpressionSyntax assignmentExpressionSyntax:
                {
                    var left = assignmentExpressionSyntax.Left;
                    var operatorToken = assignmentExpressionSyntax.OperatorToken;
                    var right = assignmentExpressionSyntax.Right;
                    var leftValue = wandaBlock.ResolveLValue(result, left, true);
                    var rightValue = wandaBlock.ResolveValue(result, right, true);
                    AssignmentBase assignment = null;

                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.EqualsToken:
                            assignment = new SimpleAssignment(leftValue, rightValue);
                            break;
                        case SyntaxKind.SlashEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryDivideOperator.Instance);
                            break;
                        case SyntaxKind.PlusEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryAddOperator.Instance);
                            break;
                        case SyntaxKind.MinusEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinarySubtractOperator.Instance);
                            break;
                        case SyntaxKind.AsteriskEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryMultiplyOperator.Instance);
                            break;
                        case SyntaxKind.PercentEqualsToken:
                            assignment =
                                new OperatorAssignment(leftValue, rightValue, BinaryRemainderOperator.Instance);
                            break;
                        case SyntaxKind.AmpersandEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryBitAndOperator.Instance);
                            break;
                        case SyntaxKind.CaretEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryBitXorOperator.Instance);
                            break;
                        case SyntaxKind.BarEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue, BinaryBitOrOperator.Instance);
                            break;
                        case SyntaxKind.LessThanLessThanEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue,
                                BinaryBitShiftLeftOperator.Instance);
                            break;
                        case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                            assignment = new OperatorAssignment(leftValue, rightValue,
                                BinaryBitShiftRightOperator.Instance);
                            break;
                        case SyntaxKind.QuestionQuestionEqualsToken:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (assignment != null)
                    {
                        result.Add(assignment);
                    }
                    else
                    {
                        Log.Debug("{Left} {Operator} {Right}", left,
                            operatorToken.Text,
                            right);
                    }

                    break;
                }

                case PrefixUnaryExpressionSyntax unaryExpressionSyntax:
                {
                    var operand = unaryExpressionSyntax.Operand;
                    var leftValue = wandaBlock.ResolveLValue(result, operand, true);
                    AssignmentBase assignment = null;
                    var operatorToken = unaryExpressionSyntax.OperatorToken;
                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.PlusToken:
                        case SyntaxKind.MinusToken:
                        case SyntaxKind.TildeToken:
                        case SyntaxKind.ExclamationToken:
                            break;
                        case SyntaxKind.PlusPlusToken:
                            assignment = new OperatorAssignment(leftValue, IntegerLiteral.One,
                                BinaryAddOperator.Instance);
                            break;
                        case SyntaxKind.MinusMinusToken:
                            assignment = new OperatorAssignment(leftValue, IntegerLiteral.One,
                                BinarySubtractOperator.Instance);
                            break;
                        case SyntaxKind.AmpersandToken:
                        case SyntaxKind.AsteriskToken:
                        case SyntaxKind.CaretToken:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (assignment != null)
                    {
                        result.Add(assignment);
                    }
                    else
                    {
                        Log.Debug("{Operator} {Right}", operatorToken.Text,
                            unaryExpressionSyntax.Operand);
                    }

                    break;
                }

                case PostfixUnaryExpressionSyntax unaryExpressionSyntax:
                {
                    var operand = unaryExpressionSyntax.Operand;
                    var leftValue = wandaBlock.ResolveLValue(result, operand, true);
                    AssignmentBase assignment = null;
                    var operatorToken = unaryExpressionSyntax.OperatorToken;
                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.PlusPlusToken:
                            assignment = new OperatorAssignment(leftValue, IntegerLiteral.One,
                                BinaryAddOperator.Instance);
                            break;
                        case SyntaxKind.MinusMinusToken:
                            assignment = new OperatorAssignment(leftValue, IntegerLiteral.One,
                                BinarySubtractOperator.Instance);
                            break;
                        case SyntaxKind.ExclamationToken:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (assignment != null)
                    {
                        // todo: that's not right. The value returned by expression is not as simple
                        result.Add(assignment);
                    }
                    else
                    {
                        Log.Debug("{Left} {Operator}", unaryExpressionSyntax.Operand,
                            operatorToken.Text);
                    }

                    break;
                }

                case InvocationExpressionSyntax invocationExpressionSyntax:
                {
                    var what = invocationExpressionSyntax.Expression.ToString();

                    FunctionCallParameter Selector(ArgumentSyntax x)
                    {
                        var xRefKindKeyword = x.RefKindKeyword.Kind();
                        var (isRef, isOut) = (
                            xRefKindKeyword == SyntaxKind.RefKeyword,
                            xRefKindKeyword == SyntaxKind.OutKeyword);

                        return new FunctionCallParameter
                        {
                            Value = wandaBlock.ResolveValue(result, x.Expression, true),
                            Out = isOut,
                            Ref = isRef
                        };
                    }

                    var with = invocationExpressionSyntax.ArgumentList.Arguments.Select(Selector);

                    var functionCallOperator = new FunctionCallOperator {Name = what};
                    functionCallOperator.Arguments.AddRange(with);

                    result.Add(functionCallOperator);
                    break;
                }

                case IdentifierNameSyntax identifierNameSyntax:
                {
                    result.Add(wandaBlock.ResolveVariableByName(identifierNameSyntax.Identifier.Text));
                    break;
                }

                case LiteralExpressionSyntax literalExpressionSyntax:
                {
                    result.Add(ResolveLiteral(literalExpressionSyntax));
                    break;
                }

                case ElementAccessExpressionSyntax elementAccessExpressionSyntax:
                {
                    result.Add(
                        ResolveRoslynElementAccessExpressionToWandaOperator(elementAccessExpressionSyntax,
                            result,
                            wandaBlock));
                    break;
                }

                case BinaryExpressionSyntax binaryExpressionSyntax:
                {
                    var operatorToken = binaryExpressionSyntax.OperatorToken;
                    PureBase operatorBase = null;
                    LogicBase logicBase = null;
                    switch (operatorToken.Kind())
                    {
                        case SyntaxKind.PlusToken:
                            operatorBase = BinaryAddOperator.Instance;
                            break;
                        case SyntaxKind.MinusToken:
                            operatorBase = BinarySubtractOperator.Instance;
                            break;
                        case SyntaxKind.AsteriskToken:
                            operatorBase = BinaryMultiplyOperator.Instance;
                            break;
                        case SyntaxKind.SlashToken:
                            operatorBase = BinaryDivideOperator.Instance;
                            break;
                        case SyntaxKind.PercentToken:
                            operatorBase = BinaryRemainderOperator.Instance;
                            break;
                        case SyntaxKind.LessThanLessThanToken:
                            operatorBase = BinaryBitShiftLeftOperator.Instance;
                            break;
                        case SyntaxKind.GreaterThanGreaterThanToken:
                            operatorBase = BinaryBitShiftRightOperator.Instance;
                            break;
                        case SyntaxKind.BarBarToken:
                            logicBase = OrLogic.Instance;
                            break;
                        case SyntaxKind.AmpersandAmpersandToken:
                            logicBase = AndLogic.Instance;
                            break;
                        case SyntaxKind.BarToken:
                            operatorBase = BinaryBitOrOperator.Instance;
                            break;
                        case SyntaxKind.AmpersandToken:
                            operatorBase = BinaryBitAndOperator.Instance;
                            break;
                        case SyntaxKind.CaretToken:
                            operatorBase = BinaryBitXorOperator.Instance;
                            break;
                        case SyntaxKind.EqualsEqualsToken:
                            logicBase = EqualLogic.Instance;
                            break;
                        case SyntaxKind.ExclamationEqualsToken:
                            logicBase = NotEqualLogic.Instance;
                            break;
                        case SyntaxKind.LessThanToken:
                            logicBase = LessLogic.Instance;
                            break;
                        case SyntaxKind.LessThanEqualsToken:
                            logicBase = LessOrEqualLogic.Instance;
                            break;
                        case SyntaxKind.GreaterThanToken:
                            logicBase = GreaterLogic.Instance;
                            break;
                        case SyntaxKind.GreaterThanEqualsToken:
                            logicBase = GreaterOrEqualLogic.Instance;
                            break;
                        case SyntaxKind.IsKeyword:
                        case SyntaxKind.AsKeyword:
                        case SyntaxKind.QuestionQuestionToken:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (operatorBase != null, logicBase != null)
                    {
                        case (true, false):
                        {
                            var left = wandaBlock.ResolveValue(result, binaryExpressionSyntax.Left, true);
                            var right = wandaBlock.ResolveValue(result, binaryExpressionSyntax.Right, true);

                            var dummy = new DummyVariable
                            {
                                TypeRef = ApproximateResultTypeOfBinaryOperator(left, right)
                            };
                            wandaBlock.LocalVariables.Add(dummy);

                            var assignment = new SimpleAssignment(dummy, left);
                            result.Add(assignment);

                            var operatorAssignment = new OperatorAssignment(dummy, right, operatorBase);
                            result.Add(operatorAssignment);
                            break;
                        }

                        case (false, true):
                        {
                            var left = wandaBlock.ResolveValue(result, binaryExpressionSyntax.Left, true);
                            var right = wandaBlock.ResolveValue(result, binaryExpressionSyntax.Right, true);

                            var operatorAssignment = new BinaryLogicExpression(logicBase, left, right);
                            result.Add(operatorAssignment);
                            break;
                        }

                        case (false, false):
                        {
                            Log.Debug("BINARY: {Left} {Operator} {Right} ({Kind})",
                                binaryExpressionSyntax.Left,
                                operatorToken.Text,
                                binaryExpressionSyntax.Right,
                                operatorToken.Kind());
                            break;
                        }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                }

                case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                {
                    result.AddRange(ConvertRoslynExpressionToWandaExpression(wandaBlock,
                        parenthesizedExpressionSyntax.Expression));
                    break;
                }

                case CastExpressionSyntax castExpressionSyntax:
                {
                    var value = wandaBlock.ResolveValue(result, castExpressionSyntax.Expression);
                    var type = ResolveType(castExpressionSyntax.Type);

                    var dummy = new DummyVariable {TypeRef = type};
                    wandaBlock.LocalVariables.Add(dummy);

                    var assignment = new SimpleAssignment(dummy, value);
                    result.Add(assignment);
                    break;
                }

                case ArrayCreationExpressionSyntax arrayCreationExpressionSyntax:
                {
                    var arrayTypeSyntax = arrayCreationExpressionSyntax.Type;
                    var type = ResolveType(arrayTypeSyntax.ElementType);
                    var ranks = arrayTypeSyntax.RankSpecifiers;
                    var initializerExpressionSyntax = arrayCreationExpressionSyntax.Initializer;
                    var initializerExpressions = initializerExpressionSyntax?.Expressions;
                    var initializerExpressionsConverted = initializerExpressions.GetValueOrDefault()
                        .Select((x, index) => (index, wandaBlock.ResolveValue(result, x)))
                        .ToArray();
                    foreach (var (arrayRankSpecifierSyntax, rankIndex) in ranks.Select((x, index) => (x, index)))
                    {
                        var sizes = arrayRankSpecifierSyntax.Sizes;
                        foreach (var syntax in sizes)
                        {
                            switch (syntax)
                            {
                                case OmittedArraySizeExpressionSyntax _ when initializerExpressionSyntax == null:
                                    Log.Error(
                                        "Roslyn ▷ Wanda Expression: {Statement} ({Kind}) when array initializer is null",
                                        syntax,
                                        syntax.Kind());
                                    throw new ArgumentException();
                                case OmittedArraySizeExpressionSyntax _:
                                    type = new ArrayTypeRef(type);
                                    if (rankIndex == 0)
                                    {
                                        var arrayType = (ArrayTypeRef) type;
                                        arrayType.Length = IntegerLiteral.From(initializerExpressionsConverted.Length);
                                    }

                                    break;
                                case { } rankExpressionSyntax:
                                    var rank = wandaBlock.ResolveValue(result, rankExpressionSyntax);
                                    type = new ArrayTypeRef(type) {Length = rank};
                                    break;
                            }
                        }
                    }

                    var arrayVariable = new DummyVariable {TypeRef = type};

                    foreach (var (index, rValue) in initializerExpressionsConverted)
                    {
                        var assignment = new SimpleAssignment(
                            new SubscriptOperator(arrayVariable, IntegerLiteral.From(index)),
                            rValue);
                        result.Add(assignment);
                    }

                    result.Add(arrayVariable);
                    break;
                }

                default:
                {
                    Log.Debug("Roslyn ▷ Wanda Expression: {Statement} ({Kind})", expressionSyntax,
                        expressionSyntax.Kind());

                    break;
                }
            }

            return result;
        }

        private static TypeReference ApproximateResultTypeOfBinaryOperator(IValue left, IValue right)
        {
            var leftType = GuessTypeFromValue(left);
            var rightType = GuessTypeFromValue(right);
            if (Equals(leftType, rightType))
                return leftType;
            return leftType ?? rightType;

            TypeReference GuessTypeFromValue(IValue value)
            {
                return value is ILValue lValue
                    ? lValue.TypeRef
                    : value is LiteralBase literal
                        ? PrimitiveTypeRef.FromLiteral(literal)
                        : null;
            }
        }
    }
}