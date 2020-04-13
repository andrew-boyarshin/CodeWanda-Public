using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public interface IDataValueInterval : IDataValue
    {
        [NotNull] ValueInterval Interval { get; }
    }
}