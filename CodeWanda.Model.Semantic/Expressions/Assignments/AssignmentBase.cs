using System;
using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Assignments
{
    public class AssignmentBase : ExpressionBase
    {
        public AssignmentBase([NotNull] ILValue left, [NotNull] IValue right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public ILValue Left { get; }
        public IValue Right { get; }
    }
}