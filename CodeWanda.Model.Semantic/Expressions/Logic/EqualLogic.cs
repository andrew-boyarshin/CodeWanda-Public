namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class EqualLogic : LogicBase
    {
        public static readonly EqualLogic Instance = new EqualLogic();

        private EqualLogic()
        {
        }

        public override string ToString()
        {
            return "==";
        }

        public override LogicBase Flip() => Instance;

        public override LogicBase Negate() => NotEqualLogic.Instance;
    }
}