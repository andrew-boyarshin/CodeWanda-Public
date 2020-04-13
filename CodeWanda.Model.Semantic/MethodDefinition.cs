using System;
using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Statements;
using CodeWanda.Model.Semantic.Types;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic
{
    public class MethodDefinition
    {
        [NotNull] public string Name { get; set; }
        [NotNull] public string Code { get; set; }
        public string Path { get; set; }

        [NotNull]
        [ItemNotNull]
        public List<MethodParameterDefinition> Arguments { get; } = new List<MethodParameterDefinition>();

        [NotNull] public SimpleCompoundStatement Body { get; } = new SimpleCompoundStatement();

        public class MethodParameterDefinition
        {
            public MethodParameterDefinition([NotNull] string name)
            {
                Variable = new ParameterVariable(this)
                {
                    Name = name ?? throw new ArgumentNullException(nameof(name))
                };
            }

            [NotNull] public TypeReference TypeRef => Variable.TypeRef;
            [NotNull] public string Name => Variable.Name;
            [NotNull] public ParameterVariable Variable { get; }
            [CanBeNull] public IValue DefaultValue { get; set; }
            public bool Ref { get; set; }
            public bool Out { get; set; }

            public override string ToString()
            {
                var data = new List<string> {TypeRef.ToString(), Name};
                if (Ref)
                    data.Insert(0, "ref");
                if (Out)
                    data.Insert(0, "out");
                return string.Join(' ', data);
            }
        }

        public override string ToString()
        {
            // todo: Not Void!
            return $"void {Name}({string.Join(", ", Arguments)})\n{Body}";
        }
    }
}