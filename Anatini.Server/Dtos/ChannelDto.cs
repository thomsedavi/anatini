namespace Anatini.Server.Dtos
{
    public class ChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? About { get; set; }
        public required string Handle { get; set; }
    }
}
