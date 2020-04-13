using System;

namespace CodeWanda.Model.Semantic
{
    public class SourcePointLocation
    {
        public Guid SourceFileId { get; set; }
        public uint Line { get; set; }
        public uint Position { get; set; }
    }
}