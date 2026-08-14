namespace Anatini.Server.Dtos
{
    public class WebsiteDto
    {
        public required Guid Id { get; set; }
        public string? Handle { get; set; }
        public string? Article { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public DateTime? PublishedAtNz { get; set; }
        public UserHeaderDto? UserHeader { get; set; }
        public SpaceHeaderDto? SpaceHeader { get; set; }
        public bool? HasBookmarked { get; set; }
        public bool? HasStarred { get; set; }
        public bool? HasDismissed { get; set; }
    }
}
