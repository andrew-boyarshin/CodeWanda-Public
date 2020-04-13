namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public class NullLiteral : LiteralBase
    {
        public static readonly NullLiteral Instance = new NullLiteral();

        public override string ToString()
        {
            return "null";
        }
    }
}