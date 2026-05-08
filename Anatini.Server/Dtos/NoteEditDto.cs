namespace Anatini.Server.Dtos
{
    public class NoteEditDto
    {
        public required Guid Id { get; set; }
        public string? Handle { get; set; }
        public required string Article { get; set; }
        public required DateTime PublishedAtUtc { get; set; }
        public required string Visibility { get; set; }
    }
}
