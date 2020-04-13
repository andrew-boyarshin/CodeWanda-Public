using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class CombinedVariant : VariableValueVariantBase
    {
        public CombinedVariant([NotNull] IDataValue value, [NotNull] Variable variable,
            [NotNull] IReadOnlyList<ArrayVariant> source, [NotNull] AnalysisContext context)
            : base(variable, context)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Storage = new CombinedArrayVariant(source);
        }

        public CombinedVariant([NotNull] IDataValue value, [NotNull] Variable variable,
            [NotNull] IReadOnlyList<VariableVariant> source, [NotNull] AnalysisContext context)
            : base(variable, context)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Storage = new CombinedVariableVariant(source);
        }

        [NotNull]
        public VariableValueVariantBase CreateNewInstance([NotNull] AnalysisContext context)
        {
            var variantBase = Storage.Create(context);
            variantBase.Value = Value;
            return variantBase;
        }

        private CombinedVariantBase Storage { get; }

        public bool IsVariable => Storage is CombinedVariableVariant;
        public bool IsArray => Storage is CombinedArrayVariant;

        public override string ToString()
        {
            return Value.ToString();
        }

        private abstract class CombinedVariantBase
        {
            public abstract VariableValueVariantBase Create([NotNull] AnalysisContext context);
        }

        private class CombinedVariableVariant : CombinedVariantBase
        {
            public CombinedVariableVariant([NotNull] [ItemNotNull] IReadOnlyList<VariableVariant> values)
            {
                Values = values ?? throw new ArgumentNullException(nameof(values));
            }

            [NotNull] [ItemNotNull] public IReadOnlyList<VariableVariant> Values { get; }

            public override VariableValueVariantBase Create(AnalysisContext context) =>
                new VariableVariant(Values[0], context);
        }

        private class CombinedArrayVariant : CombinedVariantBase
        {
            public CombinedArrayVariant([NotNull] [ItemNotNull] IReadOnlyList<ArrayVariant> values)
            {
                Values = values ?? throw new ArgumentNullException(nameof(values));
            }

            [NotNull] [ItemNotNull] public IReadOnlyList<ArrayVariant> Values { get; }

            public override VariableValueVariantBase Create(AnalysisContext context)
            {
                var arrayVariant = Values[0].Clone(context);
                foreach (var index in arrayVariant.IndexRange.Iterate(IntegerLiteral.One))
                {
                    var variants = Values.Select(x => x[context, index]).ToArray();
                    var itemVariant = variants[0].Clone(context);
                    itemVariant.Value = ValueUtilities.IterateValues(
                        variants.Select(x => x.Value),
                        ValueUtilities.IterateValuesOrFunc);

                    if (!Equals(itemVariant.Value, arrayVariant[context, index].Value))
                    {
                        arrayVariant[context, index] = itemVariant;
                    }
                }

                return arrayVariant;
            }
        }
    }
}