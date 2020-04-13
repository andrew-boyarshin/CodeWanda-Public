using CodeWanda.Model.Semantic.Expressions.Literals;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values
{
    public interface IDataValueLiteral : IDataValue
    {
        [NotNull] LiteralBase Literal { get; }
    }
}