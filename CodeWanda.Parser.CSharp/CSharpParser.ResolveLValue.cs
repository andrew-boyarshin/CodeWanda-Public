using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static ILValue ResolveLValue(this SimpleCompoundStatement block,
            List<ExpressionBase> expressionQueue,
            ExpressionSyntax expressionSyntax,
            bool suppressDummyVariableCreation = false,
            ILValue replaceCreatedDummyVariableWith = null)
        {
            var last = ResolveValueToQueue(block,
                expressionQueue,
                expressionSyntax,
                suppressDummyVariableCreation,
                replaceCreatedDummyVariableWith);
            switch (last)
            {
                case null:
                    return null;
                case ILValue value:
                    return value;
                default:
                    Log.Debug("ResolveLValue: {Expression} ({Kind}) → ILValue ({Kind})", expressionSyntax,
                        expressionSyntax.Kind(), last);
                    return null;
            }
        }
    }
}