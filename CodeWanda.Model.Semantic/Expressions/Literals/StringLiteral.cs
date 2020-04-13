namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    /// String literal is not actually an rvalue in C++ (it is lvalue, however stupid this is)
    public class StringLiteral : LiteralBase
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}