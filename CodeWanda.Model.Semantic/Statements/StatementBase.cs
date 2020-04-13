using Newtonsoft.Json;

namespace CodeWanda.Model.Semantic.Statements
{
    public abstract class StatementBase
    {
        [JsonIgnore] public MethodDefinition ParentMethod { get; set; }

        [JsonIgnore] public SimpleCompoundStatement ParentBlock { get; set; }

        public SourceRangeLocation Location { get; set; }
    }
}