using Anatini.Server.Enums;
using NpgsqlTypes;

namespace Anatini.Server.Context.Entities
{
    public class Content
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required ContentType Type { get; set; }
        public required string Handle { get; set; }
        public DateTime? PublishedAtNz { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public string? Header { get; set; }
        public required string Article { get; set; }
        public NpgsqlTsVector SearchVector { get; set; } = null!;
        public int? CurrentVersionNumber { get; set; }
        public required string ConcurrencyStamp { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<ContentImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserContentRelationship> UserRelationships { get; set; } = [];
        public virtual ICollection<ContentVersion> Versions { get; set; } = [];
    }

    public class Post : Content
    {
    }

    public class Work : Content
    {
    }

    public class EventSeries : Content
    {
        public required DateTime StartsAtNz { get; set; }
        public DateTime? EndsAtNz { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? RecurrenceRule { get; set; }
        public DateTime? ExpiresAtNz { get; set; }

        public virtual ICollection<EventException> Exceptions { get; set; } = [];
        public virtual ICollection<EventInstance> Instances { get; set; } = [];
    }

    public class EventException
    {
        public required Guid EventSeriesId { get; set; }
        public required DateTime TargetStartsAtNz { get; set; }
        public required bool IsCancelled { get; set; }
        public string? OverrideHeader { get; set; }
        public string? OverrideArticle { get; set; }
        public DateTime? OverrideStartsAtNz { get; set; }
        public TimeSpan? OverrideDuration { get; set; }
        public DateTime? OverrideEndsAtNz { get; set; }

        public virtual EventSeries Series { get; set; } = null!;
    }

    public class EventInstance
    {
        public required Guid Id { get; set; }
        public required Guid EventSeriesId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required string Header { get; set; }
        public required string Article { get; set; }
        public required DateTime StartsAtNz { get; set; }
        public required DateTime EndsAtNz { get; set; }

        public virtual EventSeries Series { get; set; } = null!;
        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<ApplicationUserEventInstanceRelationship> UserRelationships { get; set; } = [];
    }

    public class ContentVersion
    {
        public required Guid ContentId { get; set; }
        public required int VersionNumber { get; set; }
        public required string Article { get; set; }
        public required string ConcurrencyStamp { get; set; }
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
