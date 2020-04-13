namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class GreaterLogic : LogicBase
    {
        public static readonly GreaterLogic Instance = new GreaterLogic();

        private GreaterLogic()
        {
        }

        public override string ToString()
        {
            return ">";
        }

        public override LogicBase Flip() => LessLogic.Instance;

        public override LogicBase Negate() => LessOrEqualLogic.Instance;
    }
}