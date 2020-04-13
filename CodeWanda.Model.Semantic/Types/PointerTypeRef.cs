using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Types
{
    public class PointerTypeRef : QualifierTypeRef
    {
        public override string ToString()
        {
            return $"{Inner}*";
        }

        public PointerTypeRef([NotNull] TypeReference inner) : base(inner)
        {
        }
    }
}