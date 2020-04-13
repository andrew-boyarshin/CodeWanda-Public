using System;
using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class ArrayVariant : VariableValueVariantBase
    {
        [NotNull] public ValueInterval IndexRange { get; }

        public ArrayVariant([NotNull] Variable value, [NotNull] AnalysisContext context) : base(value, context)
        {
            Value = new DataValueArray(Array.Empty<LogicExpressionBase>());
            IndexRange = ValueInterval.Empty;

            if (!(value.TypeRef is ArrayTypeRef arrayTypeRef))
            {
                Log.Error("ArrayVariant expects ArrayTypeRef, not {Type}", value.TypeRef);
                return;
            }

            if (!(arrayTypeRef.Length is OrderedLiteralBase length))
            {
                Log.Information("ArrayVariant expects OrderedLiteralBase as length, not {Type}",
                    arrayTypeRef.Length);
                return;
            }

            IndexRange = new ValueInterval(IntegerLiteral.Zero, length, toInclusive: false);
            foreach (var i in IndexRange.Iterate(IntegerLiteral.One))
            {
                var itemVariant = new ArrayItemVariant(value,
                    InnerTypeRef,
                    ValueInterval.Single(i),
                    context);
                this[context, i] = itemVariant;
            }
        }

        public TypeReference InnerTypeRef
        {
            get
            {
                switch (StorageVariable.TypeRef)
                {
                    case ArrayTypeRef arrayTypeRef:
                        return arrayTypeRef.Inner;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private ArrayVariant([NotNull] ArrayVariant clone, [NotNull] AnalysisContext context) : base(clone, context)
        {
            if (clone == null) throw new ArgumentNullException(nameof(clone));
            Value = clone.Value;
            IndexRange = clone.IndexRange;
        }

        public ArrayVariant Clone([NotNull] AnalysisContext context)
        {
            return new ArrayVariant(this, context);
        }

        public ItemValueVariantBase ResolveToSingle(AnalysisContext context, IEnumerable<ItemValueVariantBase> variants)
        {
            var variantsArray = variants.ToArray();
            switch (variantsArray.Length)
            {
                case 0:
                    Log.Debug("Unexpected 0 variants");
                    return null;
                case 1:
                    return variantsArray[0];
                default:
                {
                    var valueInterval = ValueUtilities.IterateValues(variantsArray.Select(x => x.Value),
                        ValueUtilities.IterateValuesOrFunc);

                    var indexes = variantsArray.Select(x => x.Index).Aggregate((current, next) => current | next);

                    var combinedVariant = new CombinedItemVariant(
                        valueInterval,
                        StorageVariable,
                        InnerTypeRef,
                        indexes,
                        variantsArray,
                        context);

                    return combinedVariant;
                }
            }
        }

        public IEnumerable<ItemValueVariantBase> this[AnalysisContext context, IValue index]
        {
            get
            {
                switch (index)
                {
                    case IntegerLiteral integerIndex:
                    {
                        yield return this[context, integerIndex];
                        break;
                    }

                    case ArrayItemVariant arrayItemVariant:
                    {
                        yield return (ItemValueVariantBase) context[arrayItemVariant];
                        break;
                    }

                    default:
                        var range = context.ResolveToRange(index);
                        foreach (var variant in this[context, range])
                            yield return variant;
                        break;
                }
            }
            set
            {
                switch (index)
                {
                    case IntegerLiteral integerIndex:
                    {
                        var values = value.ToArray();
                        if (values.Length != 1)
                        {
                            Log.Warning("Unexpected {Count} elements when key is {Integer}",
                                values.Length, index);
                        }

                        this[context, integerIndex] = values[0];
                        break;
                    }

                    case ArrayItemVariant arrayItemVariant:
                    {
                        var values = value.ToArray();
                        if (values.Length != 1)
                        {
                            Log.Warning("Unexpected {Count} elements when key is {Index}",
                                values.Length, index);
                        }

                        context[arrayItemVariant] = values[0];
                        break;
                    }

                    default:
                        var range = context.ResolveToRange(index);
                        this[context, range] = value;
                        break;
                }
            }
        }

        public IEnumerable<ItemValueVariantBase> this[AnalysisContext context, IDataValue item]
        {
            get
            {
                var range = context.ResolveToRange(item);
                return this[context, range];
            }
            set
            {
                var range = context.ResolveToRange(item);
                this[context, range] = value;
            }
        }

        public IEnumerable<ItemValueVariantBase> this[[NotNull] AnalysisContext context,
            [CanBeNull] ValueInterval interval]
        {
            get
            {
                if (interval != null)
                    interval &= IndexRange;
                else
                    interval = IndexRange;

                foreach (var i in interval.Iterate(IntegerLiteral.One))
                {
                    yield return this[context, i];
                }
            }
            set
            {
                if (interval != null)
                    interval &= IndexRange;
                else
                    interval = IndexRange;

                var indexes = interval.Iterate(IntegerLiteral.One);
                var values = value;

                var indexesArray = indexes.ToArray();
                var valuesArray = values.ToArray();

                if (indexesArray.Length != valuesArray.Length)
                {
                    Log.Warning("Mismatch: {Indexes} = {Values}", indexesArray, valuesArray);
                }

                indexes = indexesArray;
                values = valuesArray;

                foreach (var (i, item) in indexes.Zip(values))
                {
                    this[context, i] = item;
                }
            }
        }

        public ItemValueVariantBase this[AnalysisContext context, IntegerLiteral index]
        {
            get => context[this, index];
            set => context[this, index] = value;
        }

        public override string ToString()
        {
            return $"{StorageVariable}[{IndexRange}]";
        }
    }
}