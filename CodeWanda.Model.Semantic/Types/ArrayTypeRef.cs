using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Types
{
    public class ArrayTypeRef : QualifierTypeRef
    {
        public IValue Length { get; set; }

        public override string ToString()
        {
            return Length != null ? $"{Inner}[{Length}]" : $"{Inner}[]";
        }

        public ArrayTypeRef([NotNull] TypeReference inner) : base(inner)
        {
        }
    }
}