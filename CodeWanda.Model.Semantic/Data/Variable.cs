using CodeWanda.Model.Semantic.Expressions;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Data
{
    public class Variable : ExpressionBase, ILValue
    {
        public virtual TypeReference TypeRef { get; set; }
        [NotNull] public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}