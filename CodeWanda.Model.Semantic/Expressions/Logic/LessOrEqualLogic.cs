namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class LessOrEqualLogic : LogicBase
    {
        public static readonly LessOrEqualLogic Instance = new LessOrEqualLogic();

        private LessOrEqualLogic()
        {
        }

        public override string ToString()
        {
            return "<=";
        }

        public override LogicBase Flip() => GreaterOrEqualLogic.Instance;

        public override LogicBase Negate() => GreaterLogic.Instance;
    }
}