using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Statements;
using CodeWanda.Model.Semantic.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static ExpressionBase ResolveValueToQueue(SimpleCompoundStatement block,
            List<ExpressionBase> expressionQueue,
            ExpressionSyntax expressionSyntax,
            bool suppressDummyVariableCreation = false,
            ILValue replaceCreatedDummyVariableWith = null)
        {
            var expressions = ConvertRoslynExpressionToWandaExpression(block, expressionSyntax)?.ToArray();
            if (expressions == null)
            {
                Log.Debug("ResolveValueInternal: {Expression} ({Kind}) → null", expressionSyntax,
                    expressionSyntax.Kind());
                return null;
            }

            if (expressions.Length == 0)
            {
                Log.Debug("ResolveValueInternal: {Expression} ({Kind}) → []", expressionSyntax,
                    expressionSyntax.Kind());
                return null;
            }

            switch (expressions[^1])
            {
                case SimpleAssignment simpleAssignment:
                    expressionQueue.AddRange(expressions);

                    return simpleAssignment.Left as ExpressionBase;
                case LiteralBase literalBase:
                    if (expressions.Length > 1)
                    {
                        Log.Warning("Multiple expressions when resolving literal {Literal}: {Expressions}", literalBase,
                            expressions[..^1]);
                    }

                    return literalBase;
                case ILValue value when suppressDummyVariableCreation:
                    expressionQueue.AddRange(expressions[..^1]);

                    return (ExpressionBase) value;
                case ILValue value when replaceCreatedDummyVariableWith != null:
                    var dummy = value as DummyVariable;

                    if (dummy?.TypeRef != null)
                    {
                        if (replaceCreatedDummyVariableWith is Variable variableReplacement)
                            variableReplacement.TypeRef = dummy.TypeRef;
                        else
                            Log.Warning("Expected Variable to assign TypeRef to, got {Value}",
                                replaceCreatedDummyVariableWith);
                    }

                    foreach (var expression in expressions[..^1])
                    {
                        var expressionBase = expression;
                        if (dummy != null)
                        {
                            expressionBase =
                                (ExpressionBase) expressionBase.ReplaceAll(dummy, replaceCreatedDummyVariableWith);
                        }

                        expressionQueue.Add(expressionBase);
                    }

                    return dummy != null ? (ExpressionBase) replaceCreatedDummyVariableWith : (ExpressionBase) value;
                case LogicExpressionBase value when suppressDummyVariableCreation:
                    expressionQueue.AddRange(expressions[..^1]);

                    return value;
                case AssignmentBase simpleAssignment when suppressDummyVariableCreation:
                    expressionQueue.AddRange(expressions);

                    return simpleAssignment.Left as ExpressionBase;
                default:
                {
                    var dummyVariable = new DummyVariable();
                    block.LocalVariables.Add(dummyVariable);
                    expressions[^1] = new SimpleAssignment(dummyVariable, expressions[^1]);

                    expressionQueue.AddRange(expressions);

                    return dummyVariable;
                }
            }
        }
    }
}