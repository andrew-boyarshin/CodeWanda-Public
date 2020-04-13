namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class AndLogic : LogicBase
    {
        public static readonly AndLogic Instance = new AndLogic();

        private AndLogic()
        {
        }

        public override string ToString()
        {
            return "&&";
        }

        public override LogicBase Flip() => Instance;

        public override LogicBase Negate() => OrLogic.Instance;
    }
}