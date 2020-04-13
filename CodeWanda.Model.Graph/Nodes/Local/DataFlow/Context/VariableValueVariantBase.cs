using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public abstract class VariableValueVariantBase : ValueVariantBase, IVariantWithStorageVariable
    {
        protected VariableValueVariantBase([NotNull] Variable storageVariable, [NotNull] AnalysisContext context)
            : base(context)
        {
            StorageVariable = storageVariable;
        }

        protected VariableValueVariantBase([NotNull] VariableValueVariantBase clone, [NotNull] AnalysisContext context)
            : base(clone, context)
        {
            StorageVariable = clone.StorageVariable;
        }

        public Variable StorageVariable { get; }

        public sealed override TypeReference TypeRef => StorageVariable.TypeRef;
    }
}