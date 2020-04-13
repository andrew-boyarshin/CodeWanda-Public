using System.Collections.Generic;
using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess;
using CodeWanda.Model.Semantic.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static SubscriptOperator ResolveRoslynElementAccessExpressionToWandaOperator(
            ElementAccessExpressionSyntax elementAccessExpressionSyntax,
            List<ExpressionBase> expressionQueue,
            SimpleCompoundStatement block)
        {
            var subscript = block.ResolveLValue(expressionQueue, elementAccessExpressionSyntax.Expression, true);
            if (subscript == null)
                return null;
            var argumentListArguments = elementAccessExpressionSyntax.ArgumentList.Arguments;
            if (argumentListArguments.Count != 1)
            {
                Log.Warning("{ArgumentList}: {Count} elements, not supported",
                    elementAccessExpressionSyntax.ArgumentList, argumentListArguments.Count);
                return null;
            }

            var argument = block.ResolveValue(expressionQueue, argumentListArguments[0].Expression, true);
            if (argument == null)
                return null;

            return new SubscriptOperator(subscript, argument);
        }
    }
}