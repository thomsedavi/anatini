using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class EventSeries
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required string Name { get; set; }
        public string? Article { get; set; }
        public string? Url { get; set; }
        public required DateTime StartsAtNz { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? EndsAtNz { get; set; }
        public string? RecurrenceRule { get; set; }
        public DateTime? ExpiresAtNz { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
        public virtual ICollection<EventException> Exceptions { get; set; } = [];
        public virtual ICollection<EventInstance> Instances { get; set; } = [];
    }

    public class EventException
    {
        public required Guid EventSeriesId { get; set; }
        public required DateTime TargetStartsAtNz { get; set; }
        public required bool IsCancelled { get; set; }
        public DateTime? OverrideStartsAtNz { get; set; }
        public TimeSpan? OverrideDuration { get; set; }
        public DateTime? OverrideEndsAtNz { get; set; }
        public string? OverrideName { get; set; }
        public string? OverrideArticle { get; set; }
        public string? OverrideUrl { get; set; }

        public virtual EventSeries Series { get; set; } = null!;
    }

    public class EventInstance
    {
        public required Guid Id { get; set; }
        public required Guid EventSeriesId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Handle { get; set; }
        public required string Name { get; set; }
        public string? Article { get; set; }
        public string? Url { get; set; }
        public required Status Status { get; set; }
        public required Visibility Visibility { get; set; }
        public required DateTime StartsAtNz { get; set; }
        public required DateTime EndsAtNz { get; set; }

        public virtual EventSeries Series { get; set; } = null!;
        public virtual ApplicationUser? User { get; set; }
        public virtual Space? Space { get; set; }
    }
}
