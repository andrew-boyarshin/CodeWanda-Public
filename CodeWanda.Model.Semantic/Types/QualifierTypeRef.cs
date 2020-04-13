using System;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Types
{
    public class QualifierTypeRef : TypeReference
    {
        public QualifierTypeRef([NotNull] TypeReference inner)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public TypeReference Inner { get; set; }
    }
}