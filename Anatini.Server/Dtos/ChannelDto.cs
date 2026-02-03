namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelContentDto>? TopContents { get; set; }
        public required string DefaultSlug { get; set; }
    }

    public class ChannelContentDto
    {
        public required string DefaultSlug { get; set; }
        public required string Name { get; set; }
    }
}
