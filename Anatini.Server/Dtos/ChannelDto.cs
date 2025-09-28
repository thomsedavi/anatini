namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required string Name { get; set; }
        public IEnumerable<ChannelPostDto>? TopPosts { get; set; }
        public required string DefaultSlug { get; set; }
    }

    public class ChannelPostDto
    {
        public required string DefaultSlug { get; set; }
        public required string Name { get; set; }
    }
}
