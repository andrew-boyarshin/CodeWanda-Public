namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class NotEqualLogic : LogicBase
    {
        public static readonly NotEqualLogic Instance = new NotEqualLogic();

        private NotEqualLogic()
        {
        }

        public override string ToString()
        {
            return "!=";
        }

        public override LogicBase Flip() => Instance;

        public override LogicBase Negate() => EqualLogic.Instance;
    }
}