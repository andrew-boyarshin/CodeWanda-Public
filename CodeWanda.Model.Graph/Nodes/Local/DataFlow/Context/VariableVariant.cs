using System;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Values;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Expressions.LogicExpressions;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public sealed class VariableVariant : VariableValueVariantBase
    {
        public VariableVariant([NotNull] Variable value, [NotNull] AnalysisContext context) : base(value, context)
        {
            switch (value)
            {
                case {} when value.TypeRef == null:
                    Value = new DataValueUnknown();
                    break;
                case {} when value.TypeRef is PrimitiveTypeRef primitiveTypeRef:
                    var typeMinValue = primitiveTypeRef.MinValue;
                    var typeMaxValue = primitiveTypeRef.MaxValue;
                    if (typeMinValue != null && typeMaxValue != null)
                    {
                        Value = new DataValueInterval(new ValueInterval(typeMinValue, typeMaxValue),
                            Array.Empty<LogicExpressionBase>());
                    }
                    else
                    {
                        Value = new DataValueUnknown();
                    }

                    break;
                case { } when value.TypeRef is ClassTypeRef:
	                Value = new DataValueUnknown();
	                break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public VariableVariant([NotNull] VariableVariant clone, [NotNull] AnalysisContext context)
            : base(clone, context)
        {
            if (clone == null) throw new ArgumentNullException(nameof(clone));
            Value = clone.Value;
        }

        public override string ToString()
        {
            return Value.Solve(StorageVariable, Context)?.ToString() ?? "?";
        }

        public VariableVariant Clone([NotNull] AnalysisContext context) => new VariableVariant(this, context);
    }
}