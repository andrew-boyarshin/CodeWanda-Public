using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Statements
{
    public class IfStatement : SimpleSelectionStatement
    {
        [CanBeNull] public SimpleCompoundStatement ElseBlock { get; set; }

        public IfStatement(SimpleCompoundStatement parent) : base(parent)
        {
        }

        public override string ToString()
        {
            var ifString = $"if ({string.Join(", ", Condition)})\n{Block}";
            if (ElseBlock != null)
            {
                ifString += $"\nelse\n{ElseBlock}";
            }

            return ifString;
        }
    }
}