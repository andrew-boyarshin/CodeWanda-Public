using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace CodeWanda.Parser.CSharp
{
    public static partial class CSharpParser
    {
        private static readonly Tokenizer<TypeNameToken> TypeNameTokenizer = new TokenizerBuilder<TypeNameToken>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.EqualTo('*'), TypeNameToken.Pointer)
            .Match(Character.EqualTo('['), TypeNameToken.LArray)
            .Match(Character.EqualTo(']'), TypeNameToken.RArray)
            .Match(Character.EqualTo('('), TypeNameToken.LParen)
            .Match(Character.EqualTo(')'), TypeNameToken.RParen)
            .Match(Identifier.CStyle, TypeNameToken.Identifier)
            .Build();

        private static TypeReference ResolveType([NotNull] TypeSyntax typeSyntax)
        {
            var name = typeSyntax.ToString();
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));

            var result = TypeNameTokenizer.TryTokenize(name);
            if (!result.HasValue)
                throw new ArgumentException(result.ToString(), nameof(name));

            var parsed = result.Value;
            TypeReference resolvedType = null;
            foreach (var token in parsed)
            {
                switch (token.Kind)
                {
                    case TypeNameToken.None:
                        throw new ArgumentException(token.ToString());
                    case TypeNameToken.Identifier:
                    {
                        var stringValue = token.Span.ToStringValue();
                        if (stringValue == "var")
                        {
                            break;
                        }

                        var primitiveTypeRef = ResolvePrimitiveTypeRef(stringValue);
                        resolvedType = primitiveTypeRef ?? new ClassTypeRef {Name = stringValue};
                        break;
                    }

                    case TypeNameToken.Pointer:
                        resolvedType =
                            new PointerTypeRef(resolvedType ?? throw new ApplicationException(typeSyntax.ToString()));
                        break;
                    case TypeNameToken.LArray:
                        break;
                    case TypeNameToken.RArray:
                        resolvedType =
                            new ArrayTypeRef(resolvedType ?? throw new ApplicationException(typeSyntax.ToString()));
                        break;
                    case TypeNameToken.LParen:
                    case TypeNameToken.RParen:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!(resolvedType is PrimitiveTypeRef))
                Log.Verbose("Type: {Type}", resolvedType);
            return resolvedType;
        }

        private static TypeReference ResolvePrimitiveTypeRef(string name)
        {
	        return name switch
	        {
		        "bool" => new PrimitiveTypeRef(PrimitiveType.Bool),
		        "byte" => new PrimitiveTypeRef(PrimitiveType.Byte, true) {Unsigned = true},
		        "sbyte" => new PrimitiveTypeRef(PrimitiveType.Byte, true) {Signed = true},
		        "char" => new PrimitiveTypeRef(PrimitiveType.Char),
		        "decimal" => new PrimitiveTypeRef(PrimitiveType.Double), // todo: fix decimal
                "double" => new PrimitiveTypeRef(PrimitiveType.Double),
		        "float" => new PrimitiveTypeRef(PrimitiveType.Float),
		        "int" => new PrimitiveTypeRef(PrimitiveType.Int, true) {Signed = true},
		        "uint" => new PrimitiveTypeRef(PrimitiveType.Int, true) {Unsigned = true},
		        "long" => new PrimitiveTypeRef(PrimitiveType.Long, true) {Signed = true},
		        "ulong" => new PrimitiveTypeRef(PrimitiveType.Long, true) {Unsigned = true},
		        "object" => new PrimitiveTypeRef(PrimitiveType.Object),
		        "short" => new PrimitiveTypeRef(PrimitiveType.Short, true) {Signed = true},
		        "ushort" => new PrimitiveTypeRef(PrimitiveType.Short, true) {Unsigned = true},
		        "string" => new PrimitiveTypeRef(PrimitiveType.String),
		        _ => null
	        };
        }

        private enum TypeNameToken
        {
            None,
            Identifier,
            Pointer,
            LArray,
            RArray,
            LParen,
            RParen
        }
    }
}