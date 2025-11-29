namespace Anatini.Server.Dtos
{
    public class ContentEditDto
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string DefaultSlug { get; set; }
        public required ContentVersionDto Version { get; set; }
        public bool? Protected { get; set; }
    }

    public class ContentVersionDto
    {
        public required string Name { get; set; }
        public IEnumerable<ContentElementDto>? Elements { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
