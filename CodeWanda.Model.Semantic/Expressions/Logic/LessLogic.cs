namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class LessLogic : LogicBase
    {
        public static readonly LessLogic Instance = new LessLogic();

        private LessLogic()
        {
        }

        public override string ToString()
        {
            return "<";
        }

        public override LogicBase Flip() => GreaterLogic.Instance;

        public override LogicBase Negate() => GreaterOrEqualLogic.Instance;
    }
}