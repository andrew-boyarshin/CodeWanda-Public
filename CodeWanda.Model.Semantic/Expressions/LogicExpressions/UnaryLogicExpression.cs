using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Logic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.LogicExpressions
{
    public class UnaryLogicExpression : LogicExpressionBase
    {
        public UnaryLogicExpression([NotNull] LogicBase @operator, [NotNull] IValue value) : base(@operator)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IValue Value { get; set; }

        public override string ToString()
        {
            return $"{Operator}{Value}";
        }

        public override LogicExpressionBase Negate()
        {
            switch (Operator)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(Operator));
            }
        }
    }
}