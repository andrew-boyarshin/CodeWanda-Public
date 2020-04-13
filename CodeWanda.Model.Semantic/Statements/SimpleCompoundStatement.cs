using System.Collections.Generic;
using CodeWanda.Model.Semantic.Data;
using CodeWanda.Model.Semantic.Utilities;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Statements
{
    public class SimpleCompoundStatement : StatementBase
    {
        [NotNull] [ItemNotNull] public List<StatementBase> Body { get; } = new List<StatementBase>();
        [NotNull] [ItemNotNull] public List<Variable> LocalVariables { get; } = new List<Variable>();

        public override string ToString()
        {
            return Extensions.BlockToString(Body);
        }
    }
}