namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required string Name { get; set; }
        public IEnumerable<ChannelPostDto>? Posts { get; set; }
        public required string Slug { get; set; }
    }

    public class ChannelPostDto
    {
        public required string Slug { get; set; }
        public required string Name { get; set; }
    }
}
