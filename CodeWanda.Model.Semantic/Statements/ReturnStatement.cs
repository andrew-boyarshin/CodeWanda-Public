using CodeWanda.Model.Semantic.Data;
using JetBrains.Annotations;

namespace CodeWanda.Model.Semantic.Statements
{
    public class ReturnStatement : StatementBase
    {
        [CanBeNull] public IValue What { get; set; }

        public override string ToString()
        {
            return What == null ? "return;" : $"return {What};";
        }
    }
}