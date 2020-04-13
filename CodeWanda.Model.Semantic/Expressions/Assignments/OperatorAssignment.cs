using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Operators.Pure;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Assignments
{
    public class OperatorAssignment : AssignmentBase
    {
        public OperatorAssignment([NotNull] ILValue left, [NotNull] IValue right, [NotNull] PureBase @operator) :
            base(left, right)
        {
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        }

        public PureBase Operator { get; }

        public override string ToString()
        {
            return $"{Left} {Operator}= {Right}";
        }
    }
}