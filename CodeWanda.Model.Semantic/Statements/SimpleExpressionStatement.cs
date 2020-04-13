using CodeWanda.Model.Semantic.Expressions;

namespace CodeWanda.Model.Semantic.Statements
{
    public class SimpleExpressionStatement : StatementBase
    {
        public ExpressionBase Expression { get; set; }

        public override string ToString()
        {
            return $"{Expression};";
        }
    }
}