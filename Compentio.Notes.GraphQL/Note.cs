namespace Compentio.Notes.GraphQL.Notes
{
    using System;

    public record Note
    {
        public string Id { get; set; }
        public long DocumentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
