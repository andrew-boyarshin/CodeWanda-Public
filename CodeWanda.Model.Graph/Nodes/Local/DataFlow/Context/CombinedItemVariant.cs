using System;
using System.Collections.Generic;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class CombinedItemVariant : ItemValueVariantBase
    {
        public CombinedItemVariant([NotNull] IDataValue value, [NotNull] Variable arrayStorage,
            [NotNull] TypeReference typeRef, [NotNull] ValueInterval index,
            [NotNull] IReadOnlyList<ItemValueVariantBase> source, [NotNull] AnalysisContext context)
            : base(arrayStorage, typeRef, context)
        {
            Index = index ?? throw new ArgumentNullException(nameof(index));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Values = source ?? throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(source));
        }

        [NotNull]
        public ItemValueVariantBase CreateNewInstance([NotNull] AnalysisContext context)
        {
            var variantBase = Values[0].Clone(context);
            variantBase.Value = Value;
            return variantBase;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        [NotNull] [ItemNotNull] private IReadOnlyList<ItemValueVariantBase> Values { get; }
    }
}