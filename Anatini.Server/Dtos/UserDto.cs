namespace Anatini.Server.Dtos
{
    public class UserDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public string? Bio { get; set; }
    }
}
