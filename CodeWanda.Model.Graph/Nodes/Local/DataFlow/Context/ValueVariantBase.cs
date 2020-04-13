using System;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public abstract class ValueVariantBase : ILValue, IValueVariant
    {
        public GraphNode[] Path { get; set; }
        public IDataValue Value { get; set; }
        [NotNull] public AnalysisContext Context { get; }

        protected ValueVariantBase([NotNull] AnalysisContext context)
        {
            Context = context;
        }

        protected ValueVariantBase([NotNull] ValueVariantBase clone, [NotNull] AnalysisContext context)
        {
            if (clone == null) throw new ArgumentNullException(nameof(clone));
            Path = clone.Path;
            Context = context;
        }

        public abstract TypeReference TypeRef { get; }

        public override string ToString() => throw new NotImplementedException();
    }
}