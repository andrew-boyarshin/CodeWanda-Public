using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;

namespace CodeWanda.Model.Semantic.Types
{
    public abstract class TypeReference
    {
        public virtual OrderedLiteralBase MinValue => null;
        public virtual OrderedLiteralBase MaxValue => null;
        public virtual IValue DefaultValue => null;
    }
}