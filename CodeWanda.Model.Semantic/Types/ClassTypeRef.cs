namespace CodeWanda.Model.Semantic.Types
{
    public class ClassTypeRef : TypeReference
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}