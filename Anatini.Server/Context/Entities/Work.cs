using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Work
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required WorkType Type { get; set; }
        public DateTime? PublishedAtNz { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required string Name { get; set; }
        public string? Article { get; set; }
        public required string Url { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<WorkImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserWorkEdge> UserEdges { get; set; } = [];
    }

    public class WorkImage
    {
        public required Guid WorkId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Work Work { get; set; } = null!;
    }
}
