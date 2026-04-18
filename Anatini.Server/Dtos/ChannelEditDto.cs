namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? About { get; set; }
        public required string Handle { get; set; }
        public required string Visibility { get; set; }
    }

    public class ChannelEditAliasDto
    {
        public required string Handle { get; set; }
    }
}
