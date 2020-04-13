namespace CodeWanda.Model.Semantic.Data
{
    public sealed class DummyVariable : Variable
    {
        private static uint _count = 1;

        public DummyVariable()
        {
            Name = $"dummy_{_count++}";
        }
    }
}