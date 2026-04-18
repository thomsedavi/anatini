namespace Anatini.Server.Dtos
{
    public class NoteDto
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Article { get; set; }
        public required DateTime PublishedAtUtc { get; set; }
    }
}
