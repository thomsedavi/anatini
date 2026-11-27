namespace Anatini.Server.Dtos
{
    public class OwnerHeaderDto
    {
        public required Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public ImageDto? IconImage { get; set; }
        public required string Handle { get; set; }
        public bool? IsFollowed { get; set; }
    }
}
