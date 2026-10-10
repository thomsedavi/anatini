namespace Anatini.Server.Dtos
{
    public class PostDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public string? Name { get; set; }
        public required string Article { get; set; }
        public required string Visibility { get; set; }
        public DateTime PublishedAtNz { get; set; }
        public UserHeaderDto? UserHeader { get; set; }
        public SpaceHeaderDto? SpaceHeader { get; set; }
        public bool? IsBookmarked { get; set; }
        public bool? IsStarred { get; set; }
        public bool? IsDismissed { get; set; }
        public bool? IsCollected { get; set; }
    }
}
