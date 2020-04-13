using System;
using CodeWanda.Model.Semantic.Expressions.Literals;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static LiteralBase ResolveLiteral(LiteralExpressionSyntax literalExpressionSyntax)
        {
            switch (literalExpressionSyntax.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                    var tokenValue = literalExpressionSyntax.Token.Value;
                    switch (tokenValue)
                    {
                        case byte v:
                            return IntegerLiteral.From(v);
                        case ushort v:
                            return IntegerLiteral.From(v);
                        case uint v:
                            return IntegerLiteral.From(v);
                        case ulong v:
                            return IntegerLiteral.From(v);
                        case sbyte v:
                            return IntegerLiteral.From(v);
                        case short v:
                            return IntegerLiteral.From(v);
                        case int v:
                            return IntegerLiteral.From(v);
                        case long v:
                            return IntegerLiteral.From(v);
                        case float v:
                            return new FloatLiteral(v);
                        case double v:
                            return new FloatLiteral(v);
                        case decimal v:
                            return new FloatLiteral(Convert.ToDouble(v));
                    }

                    break;
                case SyntaxKind.StringLiteralExpression:
                    return new StringLiteral {Value = (string) literalExpressionSyntax.Token.Value};
                case SyntaxKind.CharacterLiteralExpression:
                    return new CharLiteral((char) literalExpressionSyntax.Token.Value);
                case SyntaxKind.TrueLiteralExpression:
                    return BoolLiteral.TrueInstance;
                case SyntaxKind.FalseLiteralExpression:
                    return BoolLiteral.FalseInstance;
                case SyntaxKind.NullLiteralExpression:
                    return new NullLiteral();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
    }
}