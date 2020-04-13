using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Assignments
{
    public class SimpleAssignment : AssignmentBase
    {
        public override string ToString()
        {
            return $"{Left} = {Right}";
        }

        public SimpleAssignment([NotNull] ILValue left, [NotNull] IValue right) : base(left, right)
        {
        }
    }
}