using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Content
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required ContentType Type { get; set; }
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
        public virtual ICollection<ContentImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserContentEdge> UserEdges { get; set; } = [];
        public virtual ICollection<ContentVersion> Versions { get; set; } = [];
    }

    public class ContentVersion
    {
        public required Guid ContentId { get; set; }
        public required int VersionNumber { get; set; }
        public required string Article { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Content Content { get; set; } = null!;
    }

    public class ContentImage
    {
        public required Guid ContentId { get; set; }
        public required string Handle { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Content Content { get; set; } = null!;
    }
}
