namespace CodeWanda.Model.Semantic.Expressions.Logic
{
    public sealed class OrLogic : LogicBase
    {
        public static readonly OrLogic Instance = new OrLogic();

        private OrLogic()
        {
        }

        public override string ToString()
        {
            return "||";
        }

        public override LogicBase Flip() => Instance;

        public override LogicBase Negate() => AndLogic.Instance;
    }
}