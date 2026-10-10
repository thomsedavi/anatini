namespace Anatini.Server.Dtos
{
    public class UserDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public required string Handle { get; set; }
        public string? About { get; set; }
        public bool? IsTrusted { get; set; }
        public bool? IsFollowed { get; set; }
    }

    public class UserHeaderDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public required string Handle { get; set; }
    }
}
