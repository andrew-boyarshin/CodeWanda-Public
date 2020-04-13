using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public interface IValueVariant : ILValue
    {
        GraphNode[] Path { get; set; }
        [NotNull] IDataValue Value { get; }
    }
}