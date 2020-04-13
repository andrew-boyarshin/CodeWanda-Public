using CodeWanda.Model.Semantic.Data;

namespace CodeWanda.Model.Semantic.Statements
{
    public class SimpleDeclarationStatement : StatementBase
    {
        public Variable Variable { get; set; }

        public override string ToString()
        {
            return $"{Variable.TypeRef} {Variable};";
        }
    }
}