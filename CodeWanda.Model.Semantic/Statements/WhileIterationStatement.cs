namespace CodeWanda.Model.Semantic.Statements
{
    public class WhileIterationStatement : SimpleSelectionStatement
    {
        public WhileIterationStatement(SimpleCompoundStatement parent) : base(parent)
        {
        }

        private WhileIterationStatement(SimpleCompoundStatement block, bool tag)
        {
        }

        public static WhileIterationStatement FromBlock(SimpleCompoundStatement block)
        {
            return new WhileIterationStatement(block, true);
        }


        public override string ToString()
        {
            return $"while ({string.Join(", ", Condition)})\n{Block}";
        }
    }
}