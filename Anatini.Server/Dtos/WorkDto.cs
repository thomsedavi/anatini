namespace Anatini.Server.Dtos
{
    public class WorkDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required string Header { get; set; }
        public required string Article { get; set; }
        public required string Visibility { get; set; }
        public DateTime? PublishedAtNz { get; set; }
        public UserHeaderDto? UserHeader { get; set; }
        public SpaceHeaderDto? SpaceHeader { get; set; }
        public bool? HasBookmarked { get; set; }
        public bool? HasStarred { get; set; }
        public bool? HasDismissed { get; set; }
        public bool? HasCollected { get; set; }
    }
}
