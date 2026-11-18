using Anatini.Server.Enums;
using NpgsqlTypes;

namespace Anatini.Server.Context.Entities
{
    public class Content
    {
        public required Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ActiveChildId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required ContentType Type { get; set; }
        public required string Handle { get; set; }
        public string? Title { get; set; }
        public required string Body { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public DateTime? PublishedAtUtc { get; set; }
        public required string TimeZoneId { get; set; }
        public int? Version { get; set; }
        public required string ConcurrencyStamp { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Content? Parent { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<Content> Children { get; set; } = [];
        public virtual ICollection<ContentImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserContentRelationship> UserRelationships { get; set; } = [];
    }

    public class Note : Content
    {
    }

    public class Article : Content
    {
    }

    public class ArticleVersion : Content
    {
    }

    public class Work : Content
    {
    }

    public class WorkVersion : Content
    {
    }

    public class Event : Content
    {
        public required DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? RecurrenceRule { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }

        public virtual ICollection<EventException> Exceptions { get; set; } = [];
    }

    public class EventOccurrence : Content
    {
        public required DateTime StartsAtUtc { get; set; }
        public required DateTime EndsAtUtc { get; set; }
    }

    public class EventException
    {
        public required Guid EventId { get; set; }
        public required DateTime TargetStartsAtUtc { get; set; }
        public required bool IsCancelled { get; set; }
        public string? OverrideHeader { get; set; }
        public string? OverrideBody { get; set; }
        public DateTime? OverrideStartsAtUtc { get; set; }
        public TimeSpan? OverrideDuration { get; set; }
        public DateTime? OverrideEndsAtUtc { get; set; }

        public virtual Event Event { get; set; } = null!;
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
