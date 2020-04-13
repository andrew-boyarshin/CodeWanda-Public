using System;
using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Expressions.Operators.Other
{
    public class FunctionCallOperator : OperatorBase
    {
        [CanBeNull] public TypeReference ParentTypeRef { get; set; }
        [NotNull] public string Name { get; set; }

        [NotNull]
        [ItemNotNull]
        public List<FunctionCallParameter> Arguments { get; } = new List<FunctionCallParameter>();

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", Arguments)})";
        }
    }

    public class FunctionCallParameter : IValue
    {
        [CanBeNull] public IValue Value { get; set; }
        public bool Ref { get; set; }
        public bool Out { get; set; }

        public override string ToString()
        {
            var data = new List<string> {Value?.ToString()};
            if (Ref)
                data.Insert(0, "ref");
            if (Out)
                data.Insert(0, "out");
            return String.Join(' ', data);
        }
    }
}