namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required string Name { get; set; }
        public IEnumerable<ChannelContentDto>? TopContents { get; set; }
        public required string DefaultSlug { get; set; }
    }

    public class ChannelContentDto
    {
        public required string DefaultSlug { get; set; }
        public required string Name { get; set; }
    }

    public class ContentElementDto
    {
        public required string Tag { get; set; }
        public required int Index { get; set; }
        public string? Content { get; set; }
    }
}
