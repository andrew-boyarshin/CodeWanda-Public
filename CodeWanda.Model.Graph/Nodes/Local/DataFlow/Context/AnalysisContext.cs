using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;
using Serilog;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class AnalysisContext : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableConcurrentDictionary<Variable, VariableValueVariantBase> Values { get; } =
            new ObservableConcurrentDictionary<Variable, VariableValueVariantBase>();

        private ObservableConcurrentDictionary<(ArrayVariant, IntegerLiteral), ArrayItemVariant> ItemValues { get; } =
            new ObservableConcurrentDictionary<(ArrayVariant, IntegerLiteral), ArrayItemVariant>();

        [JetBrains.Annotations.NotNull]
        [ItemNotNull]
        private ObservableCollection<AnalysisContext> Parents { get; } = new ObservableCollection<AnalysisContext>();
        
        public ObservableConcurrentDictionary<Variable, VariableValueVariantBase> ValuesView { get; }

        public ObservableConcurrentDictionary<(ArrayVariant, IntegerLiteral), ArrayItemVariant> ItemValuesView { get; }

        public ReadOnlyObservableCollection<AnalysisContext> ParentsView { get; }
        
        [JetBrains.Annotations.NotNull]
        [ItemNotNull]
        public ObservableCollection<AnalysisContext> Children { get; } = new ObservableCollection<AnalysisContext>();

        public AnalysisContext()
        {
            ParentsView = new ReadOnlyObservableCollection<AnalysisContext>(Parents);
            //ValuesView = new ReadOnlyObservableCollection<KeyValuePair<Variable, VariableValueVariantBase>>(Values);
            //ItemValuesView = new ReadOnlyObservableCollection<KeyValuePair<(ArrayVariant, IntegerLiteral), ArrayItemVariant>>(ItemValues);
            ValuesView = Values;
            ItemValuesView = ItemValues;
            Values.CollectionChanged += ValuesOnCollectionChanged;
            ItemValues.CollectionChanged += ItemValuesOnCollectionChanged;
            Parents.CollectionChanged += ParentsOnCollectionChanged;
        }

        private void ValuesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(nameof(Values), e);
        }

        private void ItemValuesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(nameof(ItemValues), e);
        }

        private void ParentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(nameof(Parents), e);
        }

        public AnalysisContext(AnalysisContext parent) : this()
        {
            AddParent(parent);
        }

        public void AddParent(AnalysisContext parent)
        {
            Parents.Add(parent);
        }

        public IValue this[[DisallowNull] IValue key]
        {
            get
            {
                return key switch
                {
                    null => throw new ArgumentNullException(nameof(key)),
                    ILValue lValue => this[lValue],
                    _ => key
                };
            }
            set
            {
                switch (key)
                {
                    case null:
                        throw new ArgumentNullException(nameof(key));
                    case ILValue lValue when value is ValueVariantBase valueVariant:
                        this[lValue] = valueVariant;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(key));
                }
            }
        }

        public ValueVariantBase this[[DisallowNull] ILValue key]
        {
            get
            {
                switch (key)
                {
                    case null:
                        throw new ArgumentNullException(nameof(key));

                    case ItemValueVariantBase itemValueVariantBase:
                    {
                        var array = this[itemValueVariantBase.ArrayStorage];
                        return array switch
                        {
                            null => throw new ArgumentNullException(nameof(array)),
                            ArrayVariant arrayVariant => arrayVariant.ResolveToSingle(
                                this,
                                arrayVariant[this, itemValueVariantBase.Index]
                            ),
                            _ => throw new ArgumentOutOfRangeException(nameof(array))
                        };
                    }

                    case VariableValueVariantBase variableValueVariantBase:
                        return this[variableValueVariantBase.StorageVariable];
                    case Variable variable:
                        return this[variable];

                    case SubscriptOperator subscriptOperator:
                    {
                        var array = this[subscriptOperator.Array];
                        return array switch
                        {
                            null => throw new ArgumentNullException(nameof(array)),
                            ArrayVariant arrayVariant => arrayVariant.ResolveToSingle(
                                this,
                                arrayVariant[this, subscriptOperator.Index]
                            ),
                            _ => throw new ArgumentOutOfRangeException(nameof(array))
                        };
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key));
                }
            }
            set
            {
                switch (key)
                {
                    case null:
                        throw new ArgumentNullException(nameof(key));
                    case ItemValueVariantBase itemValueVariantBase:
                    {
                        var array = this[itemValueVariantBase.ArrayStorage];
                        switch (array)
                        {
                            case null:
                                throw new ArgumentNullException(nameof(array));
                            case ArrayVariant arrayVariant:
                                switch (value)
                                {
                                    case null:
                                        throw new ArgumentNullException(nameof(value));
                                    case ItemValueVariantBase itemValueVariant:
                                        arrayVariant[this, itemValueVariantBase.Index] = new[] {itemValueVariant};
                                        return;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(value));
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(array));
                        }
                    }

                    case VariableValueVariantBase variableVariant:
                        switch (value)
                        {
                            case null:
                                throw new ArgumentNullException(nameof(value));
                            case VariableValueVariantBase variableValueVariant:
                                this[variableVariant.StorageVariable] = variableValueVariant;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }

                    case Variable variable:
                        switch (value)
                        {
                            case null:
                                throw new ArgumentNullException(nameof(value));
                            case VariableValueVariantBase variableValueVariant:
                                this[variable] = variableValueVariant;
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value));
                        }

                    case SubscriptOperator subscriptOperator:
                    {
                        var array = this[subscriptOperator.Array];
                        switch (array)
                        {
                            case null:
                                throw new ArgumentNullException(nameof(array));
                            case ArrayVariant arrayVariant:
                                switch (value)
                                {
                                    case null:
                                        throw new ArgumentNullException(nameof(value));
                                    case ItemValueVariantBase itemValueVariantBase:
                                        arrayVariant[this, subscriptOperator.Index] = new[] {itemValueVariantBase};
                                        return;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(value));
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(array));
                        }
                    }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key));
                }
            }
        }

        public ItemValueVariantBase this[ArrayVariant array, IntegerLiteral index]
        {
            get
            {
                var set = new HashSet<AnalysisContext>();
                return FindItemVariant((array, index), set);
            }
            set
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index == null)
                    throw new ArgumentNullException(nameof(index));
                switch (value)
                {
                    case null:
                        throw new ArgumentNullException(nameof(value));
                    case ArrayItemVariant arrayItemVariant:
                        ItemValues[(array, index)] = arrayItemVariant;
                        break;
                    case CombinedItemVariant _:
                        goto default;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public VariableValueVariantBase this[[DisallowNull] Variable key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                var set = new HashSet<AnalysisContext>();
                var variant = FindVariant(key, set);
                if (variant != null)
                    return variant;

                switch (key.TypeRef)
                {
                    case null:
                        Log.Warning("Type is undefined for {Variable}", key);
                        this[key] = new VariableVariant(key, this);
                        break;
                    case ArrayTypeRef _:
                        this[key] = new ArrayVariant(key, this);
                        break;
                    default:
                        this[key] = new VariableVariant(key, this);
                        break;
                }

                return this[key];
            }
            set
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                Values[key] = value;
            }
        }

        private VariableValueVariantBase FindVariant(Variable key, ISet<AnalysisContext> set)
        {
            if (!set.Add(this))
            {
                return null;
            }

            if (Values.TryGetValue(key, out var value))
                return value;

            var variants = new List<VariableValueVariantBase>();

            foreach (var x in Parents)
            {
                var variant = x.FindVariant(key, set);
                if (variant != null)
                    variants.Add(variant);
            }

            switch (variants.Count)
            {
                case 0:
                    return null;
                case 1:
                    return variants[0];
                default:
                    var typeRef = variants.Select(x => x.TypeRef).FirstOrDefault(x => x != null);
                    if (typeRef != null)
                    {
                        IDataValue dataValue = null;
                        if (variants.TrueForAll(x =>
                            x is ArrayVariant || x is CombinedVariant combinedVariant && combinedVariant.IsArray))
                        {
                            dataValue = new DataValueArray(Array.Empty<LogicExpressionBase>());
                        }
                        else
                        {
                            var interval = variants[0].Value.Solve(key, this);
                            foreach (var variant in variants.Skip(1))
                            {
                                interval |= variant.Value.Solve(key, this);
                            }

                            if (interval != null)
                            {
                                dataValue = new DataValueInterval(interval, Array.Empty<LogicExpressionBase>());
                            }
                        }

                        if (dataValue != null)
                        {
                            switch (variants)
                            {
                                case {} when variants.TrueForAll(x => x is ArrayVariant):
                                    return new CombinedVariant(dataValue, key,
                                        variants.ConvertAll(x => (ArrayVariant) x),
                                        this);
                                case {} when variants.TrueForAll(x => x is VariableVariant):
                                    return new CombinedVariant(dataValue, key,
                                        variants.ConvertAll(x => (VariableVariant) x),
                                        this);
                                case {} when variants.TrueForAll(x =>
                                    x is ArrayVariant ||
                                    x is CombinedVariant combinedVariant && combinedVariant.IsArray):
                                    return new CombinedVariant(dataValue, key,
                                        variants.ConvertAll(x => (ArrayVariant) x.Clone(this)),
                                        this);
                                case {} when variants.TrueForAll(x =>
                                    x is VariableVariant ||
                                    x is CombinedVariant combinedVariant && combinedVariant.IsVariable):
                                    return new CombinedVariant(dataValue, key,
                                        variants.ConvertAll(x => (VariableVariant) x.Clone(this)),
                                        this);
                            }
                        }
                    }

                    Log.Warning("Multiple variants for {Key}: {Variants}", key, variants);
                    return null;
            }
        }

        private ItemValueVariantBase FindItemVariant((ArrayVariant array, IntegerLiteral index) key,
            ISet<AnalysisContext> set)
        {
            if (!set.Add(this))
                return null;

            if (ItemValues.TryGetValue(key, out var value))
                return value;

            var variants = new List<ItemValueVariantBase>();

            foreach (var x in Parents)
            {
                var variant = x.FindItemVariant(key, set);
                if (variant != null)
                    variants.Add(variant);
            }

            return variants.Count == 0
                ? null
                : key.array.ResolveToSingle(this, variants);
        }

        public override string ToString()
        {
            var values = Values.Select(x => $"{x.Key}={x.Value}");
            var itemValues = ItemValues.Select(x => $"{x.Key}={x.Value}");
            return string.Join(", ", values.Concat(itemValues));
        }

        [CanBeNull]
        public ValueInterval ResolveToRange(IValue source)
        {
            return this[source] switch
            {
                null => null,
                ValueInterval interval => interval,
                CombinedVariant combinedVariant => SolveCombinedVariant(combinedVariant.CreateNewInstance(this)),
                IVariantWithStorageVariable variantWithStorageVariable => variantWithStorageVariable.Value.Solve(
                    variantWithStorageVariable.StorageVariable, this),
                IValueVariant valueVariant => ResolveToRange(valueVariant.Value),
                IDataValueInterval valueInterval => valueInterval.Interval,
                OrderedLiteralBase orderedLiteral => ValueInterval.Single(orderedLiteral),
                _ => null
            };

            ValueInterval SolveCombinedVariant(IValueVariant newInstance) =>
                newInstance.Value.Solve(newInstance, this);
        }

        [CanBeNull]
        public ValueInterval ResolveToRange(IDataValue value)
        {
            return value switch
            {
                null => null,
                IDataValueInterval dataValueIntervalIndex => dataValueIntervalIndex.Interval,
                IDataValueUnknown _ => null,
                _ => null
            };
        }

        public IEnumerable<AnalysisContext> IterateParents(ISet<AnalysisContext> set = null)
        {
            set ??= new HashSet<AnalysisContext>();

            if (!set.Add(this))
            {
                yield break;
            }

            yield return this;

            foreach (var x in Parents)
            {
                foreach (var context in x.IterateParents(set))
                {
                    yield return context;
                }
            }
        }

        public IEnumerable<(ArrayVariant, IntegerLiteral)> AssignedArrayItems => ItemValues.Keys;
        public IEnumerable<Variable> AssignedVariables => Values.Keys;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(string propertyName, NotifyCollectionChangedEventArgs args)
        {
            Log.Verbose(
                "AnalysisContext[{Id}]: {CollectionName} changed: {Action}",
                RuntimeHelpers.GetHashCode(this),
                propertyName,
                args.Action
            );
            
            CollectionChanged?.Invoke(this, args);
        }
    }
}