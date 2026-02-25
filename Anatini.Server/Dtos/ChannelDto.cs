namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelPostDto>? TopPosts { get; set; }
        public required string DefaultHandle { get; set; }
    }

    public class ChannelPostDto
    {
        public required string DefaultHandle { get; set; }
        public required string Name { get; set; }
    }
}
