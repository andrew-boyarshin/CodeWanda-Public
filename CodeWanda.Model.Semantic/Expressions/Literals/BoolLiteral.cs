namespace CodeWanda.Model.Semantic.Expressions.Literals
{
    public class BoolLiteral : LiteralBase
    {
        public static readonly BoolLiteral TrueInstance = new BoolLiteral(true);
        public static readonly BoolLiteral FalseInstance = new BoolLiteral(false);

        public BoolLiteral(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}