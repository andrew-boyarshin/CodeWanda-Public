using System;
using System.ComponentModel;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Types
{
    public class PrimitiveTypeRef : TypeReference
    {
        private bool _signed;
        private bool _unsigned;
        public PrimitiveType Type { get; set; }

        public bool Signed
        {
            get => _signed;
            set
            {
                if (value && _signed)
                    return;
                if (!value && !_signed)
                    return;
                switch (Type)
                {
                    case PrimitiveType.Null:
                    case PrimitiveType.Void:
                    case PrimitiveType.Object:
                    case PrimitiveType.String:
                    case PrimitiveType.Bool:
                    case PrimitiveType.Float:
                    case PrimitiveType.Double:
                        ThrowSignException("signed");
                        break;
                    case PrimitiveType.Byte:
                    case PrimitiveType.Short:
                    case PrimitiveType.Int:
                    case PrimitiveType.Long:
                    case PrimitiveType.Char:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (value && Unsigned)
                    throw new ApplicationException("Primitive already marked as unsigned");

                _signed = value;
            }
        }

        public bool Unsigned
        {
            get => _unsigned;
            set
            {
                if (value && _unsigned)
                    return;
                if (!value && !_unsigned)
                    return;
                switch (Type)
                {
                    case PrimitiveType.Null:
                    case PrimitiveType.Void:
                    case PrimitiveType.Object:
                    case PrimitiveType.String:
                    case PrimitiveType.Bool:
                    case PrimitiveType.Float:
                    case PrimitiveType.Double:
                        ThrowSignException("unsigned");
                        break;
                    case PrimitiveType.Byte:
                    case PrimitiveType.Short:
                    case PrimitiveType.Int:
                    case PrimitiveType.Long:
                    case PrimitiveType.Char:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (value && Signed)
                    throw new ApplicationException("Primitive already marked as signed");

                _unsigned = value;
            }
        }

        public PrimitiveTypeRef(PrimitiveType type, bool noDefaultSigns = false)
        {
            if (!Enum.IsDefined(typeof(PrimitiveType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(PrimitiveType));
            Type = type;

            if (!noDefaultSigns)
            {
                switch (type)
                {
                    case PrimitiveType.Null:
                    case PrimitiveType.Void:
                    case PrimitiveType.Object:
                    case PrimitiveType.String:
                    case PrimitiveType.Bool:
                    case PrimitiveType.Float:
                    case PrimitiveType.Double:
                        break;
                    case PrimitiveType.Byte:
                    case PrimitiveType.Short:
                    case PrimitiveType.Int:
                    case PrimitiveType.Long:
                    case PrimitiveType.Char:
                        Signed = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                // todo: C# has Unsigned byte by default
            }
        }

        public override string ToString()
        {
            var stringBasic = ToStringBasic();

            switch (Type)
            {
                case PrimitiveType.Null:
                case PrimitiveType.Void:
                case PrimitiveType.Object:
                case PrimitiveType.String:
                case PrimitiveType.Bool:
                case PrimitiveType.Float:
                case PrimitiveType.Double:
                    break;
                case PrimitiveType.Byte:
                case PrimitiveType.Short:
                case PrimitiveType.Int:
                case PrimitiveType.Long:
                    break;
                case PrimitiveType.Char:
                    if (Signed)
                        return $"signed {stringBasic}";
                    else if (Unsigned)
                        return $"unsigned {stringBasic}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }

            return stringBasic;
        }

        private string ToStringBasic()
        {
            switch (Type)
            {
                case PrimitiveType.Null:
                    return "null";
                case PrimitiveType.Void:
                    return "void";
                case PrimitiveType.Byte when Signed:
                    return "sbyte";
                case PrimitiveType.Byte:
                    return "byte";
                case PrimitiveType.Short when Unsigned:
                    return "ushort";
                case PrimitiveType.Short:
                    return "short";
                case PrimitiveType.Int when Unsigned:
                    return "uint";
                case PrimitiveType.Int:
                    return "int";
                case PrimitiveType.Long when Unsigned:
                    return "ulong";
                case PrimitiveType.Long:
                    return "long";
                case PrimitiveType.Char:
                    return "char";
                case PrimitiveType.Float:
                    return "float";
                case PrimitiveType.Double:
                    return "double";
                case PrimitiveType.Object:
                    return "object";
                case PrimitiveType.Bool:
                    return "bool";
                case PrimitiveType.String:
                    return "string";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ThrowSignException(string message)
        {
            throw new ApplicationException($"{Type} cannot be {message}");
        }

        public override OrderedLiteralBase MinValue
        {
            get
            {
                switch (Type)
                {
                    case PrimitiveType.Null:
                        goto default;
                    case PrimitiveType.Void:
                        goto default;
                    case PrimitiveType.Byte when Signed:
                        return IntegerLiteral.MinSByte;
                    case PrimitiveType.Byte:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Short when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Short:
                        return IntegerLiteral.MinShort;
                    case PrimitiveType.Int when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Int:
                        return IntegerLiteral.MinInt;
                    case PrimitiveType.Long when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Long:
                        return IntegerLiteral.MinLong;
                    case PrimitiveType.Char:
                        return new CharLiteral(char.MinValue);
                    case PrimitiveType.Float:
                        return new FloatLiteral(float.MinValue);
                    case PrimitiveType.Double:
                        return new FloatLiteral(double.MinValue);
                    case PrimitiveType.Object:
                    case PrimitiveType.Bool:
                    case PrimitiveType.String:
                        goto default;
                    default:
                        return null;
                }
            }
        }

        public override OrderedLiteralBase MaxValue
        {
            get
            {
                switch (Type)
                {
                    case PrimitiveType.Null:
                        goto default;
                    case PrimitiveType.Void:
                        goto default;
                    case PrimitiveType.Byte when Signed:
                        return IntegerLiteral.MaxSByte;
                    case PrimitiveType.Byte:
                        return IntegerLiteral.MaxByte;
                    case PrimitiveType.Short when Unsigned:
                        return IntegerLiteral.MaxUShort;
                    case PrimitiveType.Short:
                        return IntegerLiteral.MaxShort;
                    case PrimitiveType.Int when Unsigned:
                        return IntegerLiteral.MaxUInt;
                    case PrimitiveType.Int:
                        return IntegerLiteral.MaxInt;
                    case PrimitiveType.Long when Unsigned:
                        return IntegerLiteral.MaxULong;
                    case PrimitiveType.Long:
                        return IntegerLiteral.MaxLong;
                    case PrimitiveType.Char:
                        return new CharLiteral(char.MaxValue);
                    case PrimitiveType.Float:
                        return new FloatLiteral(float.MaxValue);
                    case PrimitiveType.Double:
                        return new FloatLiteral(double.MaxValue);
                    case PrimitiveType.Object:
                    case PrimitiveType.Bool:
                    case PrimitiveType.String:
                        goto default;
                    default:
                        return null;
                }
            }
        }

        public override IValue DefaultValue
        {
            get
            {
                switch (Type)
                {
                    case PrimitiveType.Null:
                        return NullLiteral.Instance;
                    case PrimitiveType.Void:
                        goto default;
                    case PrimitiveType.Byte when Signed:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Byte:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Short when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Short:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Int when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Int:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Long when Unsigned:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Long:
                        return IntegerLiteral.Zero;
                    case PrimitiveType.Char:
                        return new CharLiteral(default(char));
                    case PrimitiveType.Float:
                        return new FloatLiteral(default(float));
                    case PrimitiveType.Double:
                        return new FloatLiteral(default(double));
                    case PrimitiveType.Object:
                        return NullLiteral.Instance;
                    case PrimitiveType.Bool:
                        return BoolLiteral.FalseInstance;
                    case PrimitiveType.String:
                        return NullLiteral.Instance;
                    default:
                        return null;
                }
            }
        }

        public static PrimitiveTypeRef FromLiteral(LiteralBase literalBase)
        {
            switch (literalBase)
            {
                case null:
                    return null;
                case BoolLiteral _:
                    return new PrimitiveTypeRef(PrimitiveType.Bool);
                case CharLiteral _:
                    return new PrimitiveTypeRef(PrimitiveType.Char);
                case FloatLiteral _:
                    return new PrimitiveTypeRef(PrimitiveType.Float);
                case IntegerLiteral integerLiteral:
                    if (integerLiteral < IntegerLiteral.MinInt)
                        return new PrimitiveTypeRef(PrimitiveType.Long);
                    if (integerLiteral > IntegerLiteral.MaxInt)
                        return new PrimitiveTypeRef(PrimitiveType.Long);
                    return new PrimitiveTypeRef(PrimitiveType.Int);
                case NullLiteral _:
                    return new PrimitiveTypeRef(PrimitiveType.Null);
                case StringLiteral _:
                    return new PrimitiveTypeRef(PrimitiveType.String);
                default:
                    throw new ArgumentOutOfRangeException(nameof(literalBase));
            }
        }
    }
}