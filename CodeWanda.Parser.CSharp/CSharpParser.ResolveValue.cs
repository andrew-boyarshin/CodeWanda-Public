using System;
using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Expressions.Assignments;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static IValue ResolveValue(this SimpleCompoundStatement block,
            List<ExpressionBase> expressionQueue,
            ExpressionSyntax expressionSyntax,
            bool suppressDummyVariableCreation = false,
            ILValue replaceCreatedDummyVariableWith = null)
        {
            var last = ResolveValueToQueue(block, expressionQueue, expressionSyntax, suppressDummyVariableCreation,
                replaceCreatedDummyVariableWith);
            switch (last)
            {
                case null:
                    return null;
                case AssignmentBase value:
                    return value.Left;
                case IValue value:
                    return value;
                default:
                    throw new ApplicationException();
            }
        }
    }
}