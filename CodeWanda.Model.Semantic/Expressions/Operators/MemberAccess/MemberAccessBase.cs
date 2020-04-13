using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;

namespace CodeWanda.Model.Semantic.Expressions.Operators.MemberAccess
{
    public abstract class MemberAccessBase : OperatorBase, ILValue
    {
        public virtual TypeReference TypeRef { get; set; }
    }
}