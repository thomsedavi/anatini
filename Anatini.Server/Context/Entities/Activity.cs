using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Activity
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required ActivityType Type { get; set; }
        public required DateTime PublishedAtUtc { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public string? Name { get; set; }
        public string? Article { get; set; }
        public string? Url { get; set; }
        public int? CurrentVersionNumber { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<ActivityImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserActivityEdge> UserEdges { get; set; } = [];
        public virtual ICollection<ActivityVersion> Versions { get; set; } = [];
    }

    public class ActivityVersion
    {
        public required Guid ActivityId { get; set; }
        public required int VersionNumber { get; set; }
        public required string Article { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Activity Activity { get; set; } = null!;
    }

    public class ActivityImage
    {
        public required Guid ActivityId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Activity Activity { get; set; } = null!;
    }
}
