namespace Anatini.Server.Dtos
{
    public class UserEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? About { get; set; }
        public required string Visibility { get; set; }
        public ImageDto? IconImage { get; set; }
        public required IEnumerable<ChannelEditDto> Channels { get; set; }
        public required string Handle { get; set; }
        public string? UserName { get; set; }
    }
}
