using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public abstract class ItemValueVariantBase : ValueVariantBase
    {
        public ItemValueVariantBase([NotNull] Variable array, [NotNull] TypeReference typeRef,
            [NotNull] AnalysisContext context)
            : base(context)
        {
            ArrayStorage = array ?? throw new ArgumentNullException(nameof(array));
            TypeRef = typeRef ?? throw new ArgumentNullException(nameof(typeRef));
        }

        public ItemValueVariantBase([NotNull] ItemValueVariantBase clone, [NotNull] AnalysisContext context)
            : base(clone, context)
        {
            ArrayStorage = clone.ArrayStorage;
            Index = clone.Index;
            TypeRef = clone.TypeRef;
        }

        [NotNull] public sealed override TypeReference TypeRef { get; }

        [NotNull] public Variable ArrayStorage { get; }

        [NotNull] public ValueInterval Index { get; protected set; }
    }
}