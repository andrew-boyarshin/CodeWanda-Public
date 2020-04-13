using System.Collections.Generic;
using System.Linq;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Statements;

namespace CodeWanda.Model.Semantic.Utilities
{
    public static partial class Extensions
    {
        public static Variable ResolveVariableByName(this SimpleCompoundStatement block, string name)
        {
            var variable = block.LocalVariables.FirstOrDefault(x => x.Name == name);
            return variable ?? block.ParentBlock?.ResolveVariableByName(name);
        }

        public static string BlockToString<T>(IEnumerable<T> items)
        {
            var body = items.Select(FormatLine).ToArray();
            return $"{{\n{string.Join('\n', body)}\n}}";

            string FormatLine(T x)
            {
                return string.Join('\n', x.ToString().Split('\n').Select(v => v.Insert(0, "    ")));
            }
        }
    }
}