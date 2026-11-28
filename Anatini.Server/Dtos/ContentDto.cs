namespace Anatini.Server.Dtos
{
    public class ContentDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required string Body { get; set; }
        public required string Visibility { get; set; }
        public required string Status { get; set; }
        public OwnerHeaderDto? OwnerHeader { get; set; }
        public bool? IsScheduled { get; set; }
        public bool? IsBookmarked { get; set; }
        public bool? IsLiked { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsCollected { get; set; }
        public bool? IsOwned { get; set; }
    }
}
