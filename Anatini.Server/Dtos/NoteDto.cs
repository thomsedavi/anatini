namespace Anatini.Server.Dtos
{
    public class NoteDto
    {
        public required Guid Id { get; set; }
        public string? Handle { get; set; }
        public required string Article { get; set; }
        public required DateTime PublishedAtUtc { get; set; }
        public UserHeaderDto? UserHeader { get; set; }
        public ChannelHeaderDto? ChannelHeader { get; set; }
        public bool? HasBookmarked { get; set; }
        public bool? HasStarred { get; set; }
        public bool? HasDismissed { get; set; }
    }
}
