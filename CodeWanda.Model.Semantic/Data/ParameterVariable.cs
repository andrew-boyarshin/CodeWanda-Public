using System;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Data
{
    public class ParameterVariable : Variable
    {
        public ParameterVariable([NotNull] MethodDefinition.MethodParameterDefinition parameterDefinition)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        [NotNull] public MethodDefinition.MethodParameterDefinition ParameterDefinition { get; set; }
    }
}