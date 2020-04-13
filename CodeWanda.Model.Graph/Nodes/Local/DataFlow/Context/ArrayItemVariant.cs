using System;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class ArrayItemVariant : ItemValueVariantBase
    {
        public ArrayItemVariant([NotNull] Variable arrayStorage, [NotNull] TypeReference typeRef,
            [NotNull] ValueInterval index, [NotNull] AnalysisContext context)
            : base(arrayStorage, typeRef, context)
        {
            Index = index ?? throw new ArgumentNullException(nameof(index));
            Value = new DataValueUnknown();

            switch (typeRef)
            {
                case PrimitiveTypeRef primitiveTypeRef:
                    var defaultValue = primitiveTypeRef.DefaultValue;
                    if (defaultValue is OrderedLiteralBase defaultOrderedValue)
                        Value = new DataValueInterval(ValueInterval.Single(defaultOrderedValue),
                            Array.Empty<LogicExpressionBase>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeRef));
            }
        }

        private ArrayItemVariant([NotNull] ArrayItemVariant clone, [NotNull] AnalysisContext context)
            : base(clone, context)
        {
            if (clone == null) throw new ArgumentNullException(nameof(clone));
            Value = clone.Value;
        }

        public ArrayItemVariant Clone([NotNull] AnalysisContext context)
        {
            return new ArrayItemVariant(this, context);
        }

        public override string ToString()
        {
            return Value.Solve(this, Context)?.ToString() ?? "?";
        }
    }
}