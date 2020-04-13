using System;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Logic;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.LogicExpressions
{
    public abstract class LogicExpressionBase : ExpressionBase
    {
        protected LogicExpressionBase([NotNull] LogicBase @operator)
        {
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        }

        [NotNull] public LogicBase Operator { get; set; }

        public abstract LogicExpressionBase Negate();

        protected IValue Negate(IValue value)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                case LogicExpressionBase logicExpressionBase:
                    return logicExpressionBase.Negate();
                default:
                    return value;
            }
        }
    }
}