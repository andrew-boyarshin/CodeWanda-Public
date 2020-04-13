using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static ExpressionBase ResolveValueToBlock(SimpleCompoundStatement block,
            ExpressionSyntax expressionSyntax,
            bool suppressDummyVariableCreation = false,
            ILValue replaceCreatedDummyVariableWith = null)
        {
            var expressionQueue = new List<ExpressionBase>();
            var last = ResolveValueToQueue(block,
                expressionQueue,
                expressionSyntax,
                suppressDummyVariableCreation,
                replaceCreatedDummyVariableWith);

            foreach (var expressionBase in expressionQueue)
            {
                ProcessWandaExpressionIntoWandaBlock(block, expressionBase);
            }

            return last;
        }
    }
}