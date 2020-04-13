namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class GreaterOrEqualLogic : LogicBase
    {
        public static readonly GreaterOrEqualLogic Instance = new GreaterOrEqualLogic();

        private GreaterOrEqualLogic()
        {
        }

        public override string ToString()
        {
            return ">=";
        }

        public override LogicBase Flip() => LessOrEqualLogic.Instance;

        public override LogicBase Negate() => LessLogic.Instance;
    }
}