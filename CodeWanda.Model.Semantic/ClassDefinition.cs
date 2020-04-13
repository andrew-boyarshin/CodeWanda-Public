using System.Collections.Generic;
using CodeWanda.Model.Semantic.Utilities;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic
{
    public class ClassDefinition
    {
        [NotNull] public string Name { get; set; }
        [NotNull] [ItemNotNull] public List<MethodDefinition> Methods { get; } = new List<MethodDefinition>();

        public override string ToString()
        {
            return $"class {Name}\n{Extensions.BlockToString(Methods)}";
        }
    }
}