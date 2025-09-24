namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelPostDto>? Posts { get; set; }
        public required IEnumerable<ChannelSlugDto> Slugs { get; set; }
        public required Guid? DefaultSlugId { get; set; }
    }

    public class ChannelPostDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    public class ChannelSlugDto
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
    }
}
