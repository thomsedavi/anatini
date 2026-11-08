using Anatini.Server.Enums;
using Anatini.Server.Events;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class EventContextExtensions
    {
        public static Event AddUserEvent(this ApplicationDbContext context, Guid userId, CreateEvent createEvent, Status status, string body)
        {
            var eventId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var @event = new Event
            {
                Id = eventId,
                UserId = userId,
                Status = status,
                Type = ContentType.Event,
                Visibility = createEvent.Visibility,
                Title = createEvent.Title,
                Body = body,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                StartsAtNz = createEvent.StartsAtNz,
                EndsAtNz = createEvent.EndsAtNz,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                Handle = createEvent.Handle ?? eventId.ToString(),
                PublishedAtNz = createEvent.PublishedAtNz?.Truncate()
            };

            context.Add(@event);

            return @event;
        }

        public static Event AddSpaceEvent(this ApplicationDbContext context, Guid spaceId, CreateEvent createEvent, Status status, string body)
        {
            var eventId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var @event = new Event
            {
                Id = eventId,
                SpaceId = spaceId,
                Status = Status.Published,
                Type = ContentType.Event,
                Visibility = createEvent.Visibility,
                Title = createEvent.Title,
                Body = body,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                StartsAtNz = createEvent.StartsAtNz,
                EndsAtNz = createEvent.EndsAtNz,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                Handle = eventId.ToString()
            };

            context.Add(@event);

            return @event;
        }

        public static int AddEventOccurrence(this ApplicationDbContext context, Event @event, Status status)
        {
            if (@event.RecurrenceRule == null)
            {
                var eventOccurrence = new EventOccurrence
                {
                    Id = Guid.CreateVersion7(),
                    Handle = @event.StartsAtNz.GetDate(),
                    EventId = @event.Id,
                    UserId = @event.UserId,
                    SpaceId = @event.SpaceId,
                    Title = @event.Title ?? throw new InvalidOperationException(),
                    StartsAtNz = @event.StartsAtNz,
                    EndsAtNz = @event.EndsAtNz ?? throw new InvalidOperationException(),
                    Body = @event.Body,
                    Status = status,
                    Visibility = @event.Visibility,
                    PublishedAtNz = @event.PublishedAtNz
                };

                context.Add(eventOccurrence);

                return 1;
            }
            else
            {
                var eventOccurrenceCount = 0;
                var recurrenceRule = new RecurrenceRule(@event.RecurrenceRule);

                var occurrencesNz = recurrenceRule.GetOccurrenceNz(@event.StartsAtNz, @event.EndsAtNz, @event.Duration);

                foreach (var occurrenceNz in occurrencesNz)
                {
                    var eventOccurrence = new EventOccurrence
                    {
                        Id = Guid.CreateVersion7(),
                        Handle = occurrenceNz.Item1.GetDate(),
                        EventId = @event.Id,
                        UserId = @event.UserId,
                        SpaceId = @event.SpaceId,
                        Title = @event.Title ?? throw new InvalidOperationException(),
                        StartsAtNz = occurrenceNz.Item1,
                        EndsAtNz = occurrenceNz.Item2,
                        Body = @event.Body,
                        Status = status,
                        Visibility = @event.Visibility
                    };

                    context.Add(eventOccurrence);
                    eventOccurrenceCount++;
                }

                return eventOccurrenceCount;
            }
        }
    }
}
