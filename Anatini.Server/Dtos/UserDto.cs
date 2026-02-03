namespace Anatini.Server.Dtos
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public string? About { get; set; }
    }
}
