using CodeWanda.Model.Semantic.Data;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public interface IVariantWithStorageVariable : IValueVariant
    {
        Variable StorageVariable { get; }
    }
}