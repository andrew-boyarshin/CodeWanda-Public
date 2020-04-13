using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Logic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.LogicExpressions
{
    public class BinaryLogicExpression : LogicExpressionBase
    {
        public BinaryLogicExpression([NotNull] LogicBase @operator, [NotNull] IValue left, [NotNull] IValue right) :
            base(@operator)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        [NotNull] public IValue Left { get; }
        [NotNull] public IValue Right { get; }

        public BinaryLogicExpression Flip()
        {
            var flip = Operator.Flip();
            if (flip == null)
            {
                return null;
            }

            return new BinaryLogicExpression(flip, Right, Left);
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }

        public override LogicExpressionBase Negate()
        {
            var negate = Operator.Negate();
            if (negate == null)
            {
                return null;
            }

            return new BinaryLogicExpression(negate, Negate(Left), Negate(Right));
        }
    }
}