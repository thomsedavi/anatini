namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelPostDto>? Posts { get; set; }
    }

    public class ChannelPostDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
