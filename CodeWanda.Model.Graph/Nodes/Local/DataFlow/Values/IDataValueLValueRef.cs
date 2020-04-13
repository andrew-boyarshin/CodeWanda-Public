using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public interface IDataValueLValueRef : IDataValue
    {
        [NotNull] ILValue Target { get; }
    }
}