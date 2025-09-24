namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelEditPostDto>? Posts { get; set; }
        public required IEnumerable<ChannelEditSlugDto> Slugs { get; set; }
        public required Guid? DefaultSlugId { get; set; }
    }

    public class ChannelEditPostDto
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
        public required string Name { get; set; }
    }

    public class ChannelEditSlugDto
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
    }
}
