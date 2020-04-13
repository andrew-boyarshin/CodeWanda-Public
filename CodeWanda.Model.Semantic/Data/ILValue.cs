using CodeWanda.Model.Semantic.Types;

namespace CodeWanda.Model.Semantic.Data
{
    public interface ILValue : IValue
    {
        TypeReference TypeRef { get; }
    }
}