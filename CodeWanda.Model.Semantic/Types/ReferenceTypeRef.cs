using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Types
{
    public class ReferenceTypeRef : QualifierTypeRef
    {
        public override string ToString()
        {
            return $"{Inner}&";
        }

        public ReferenceTypeRef([NotNull] TypeReference inner) : base(inner)
        {
        }
    }
}