using System;
using System.Linq;
using CodeWanda.Model.Graph.Nodes.Local;
using CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context;
using CodeWanda.Model.Semantic.Types;
using Serilog;

namespace CodeWanda.Analyzer.DataFlow
{
    public partial class IntraProceduralDataFlowAnalyzer
    {
        private void CreateMethodParameterVariables(AnalysisContext context)
        {
            foreach (var argument in MethodRoot.Arguments)
            {
                var variable = MethodRoot.ParameterVariables.FirstOrDefault(x => x.Name == argument.Name);
                if (variable == null)
                {
                    Log.Warning("No parameter variable {Name} found in {Variables}",
                        argument.Name,
                        MethodRoot.Arguments);
                    continue;
                }

                VariableValueVariantBase variant;
                switch (variable.TypeRef)
                {
	                case ClassTypeRef _:
		                variant = new VariableVariant(variable, context);
                        break;
	                case ArrayTypeRef _:
                        variant = new ArrayVariant(variable, context);
                        break;
                    case PrimitiveTypeRef _:
                        variant = new VariableVariant(variable, context);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                variant.Path = new GraphNode[0];
                context[variable] = variant;
            }
        }
    }
}