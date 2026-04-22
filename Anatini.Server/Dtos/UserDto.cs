namespace Anatini.Server.Dtos
{
    public class UserDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public required string Handle { get; set; }
        public string? About { get; set; }
    }
}
