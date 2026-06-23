using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class EventSeries
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Name { get; set; }
        public required DateTime StartsAtUtc { get; set; }
        public required TimeSpan Duration { get; set; }
        public string? RecurrenceRule { get; set; }
        public required Visibility Visibility { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidUntil { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public ApplicationUser? User { get; set; }
        public Space? Space { get; set; }
    }

    public class EventException
    {
        public required Guid EventSeriesId { get; set; }
        public required DateTime OriginalDateUtc { get; set; }
        public required bool IsCancelled { get; set; }
        public DateTime? RescheduledStartUtc { get; set; }
        public string? CustomName { get; set; }
    }

    public class EventInstance
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SpaceId { get; set; }
        public required string Name { get; set; }
        public required Guid EventSeriesId { get; set; }
        public required DateTime StartsAtUtc { get; set; }
        public required DateTime EndsAtUtc { get; set; }

        public ApplicationUser? User { get; set; }
        public Space? Space { get; set; }
    }
}
