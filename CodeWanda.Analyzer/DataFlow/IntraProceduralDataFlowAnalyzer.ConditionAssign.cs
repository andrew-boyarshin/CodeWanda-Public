using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.Literals;
using JetBrains.Annotations;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private IDataValue ConditionAssign(IValue right,
            [NotNull] AnalysisContext context,
            [NotNull] IDataValue oldValue)
        {
            var rightRange = context.ResolveToRange(right);
            switch (right)
            {
                case {} when rightRange != null:
                {
                    return new DataValueInterval(rightRange, oldValue.Constraints);
                }

                case LiteralBase rightLiteral:
                {
                    return new DataValueLiteral(rightLiteral, oldValue.Constraints);
                }

                case IVariantWithStorageVariable rightVariant:
                {
                    return new DataValueLValueRef(rightVariant.StorageVariable,
                        oldValue.Constraints);
                }
            }

            return null;
        }
    }
}