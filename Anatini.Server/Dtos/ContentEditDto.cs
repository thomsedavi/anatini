namespace Anatini.Server.Dtos
{
    public class ContentEditDto
    {
        public required string Id { get; set; }
        public required string ChannelId { get; set; }
        public required string DefaultSlug { get; set; }
        public required ContentVersionDto Version { get; set; }
        public bool? Protected { get; set; }
    }

    public class ContentVersionDto
    {
        public required string Name { get; set; }
        public string? Article { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
