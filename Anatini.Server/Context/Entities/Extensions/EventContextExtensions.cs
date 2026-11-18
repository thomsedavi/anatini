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
                StartsAtUtc = createEvent.StartsAtNz.ConvertNzToUtc(),
                EndsAtUtc = createEvent.EndsAtNz?.ConvertNzToUtc(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                Handle = createEvent.Handle ?? eventId.ToString(),
                PublishedAtUtc = createEvent.PublishedAtNz?.ConvertNzToUtc().Truncate(),
                TimeZoneId = "Pacific/Auckland"
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
                Status = status,
                Type = ContentType.Event,
                Visibility = createEvent.Visibility,
                Title = createEvent.Title,
                Body = body,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                StartsAtUtc = createEvent.StartsAtNz.ConvertNzToUtc(),
                EndsAtUtc = createEvent.EndsAtNz?.ConvertNzToUtc(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                Handle = eventId.ToString(),
                TimeZoneId = "Pacific/Auckland"
            };

            context.Add(@event);

            return @event;
        }

        public static int AddEventOccurrences(this ApplicationDbContext context, Event @event, Status status)
        {
            var utcNow = DateTime.UtcNow;

            if (@event.RecurrenceRule == null)
            {
                var eventOccurrence = new EventOccurrence
                {
                    Id = Guid.CreateVersion7(),
                    Handle = @event.StartsAtUtc.ConvertUtcToNz().GetDate(),
                    ParentId = @event.Id,
                    Type = ContentType.EventOccurrence,
                    UserId = @event.UserId,
                    SpaceId = @event.SpaceId,
                    Title = @event.Title ?? throw new InvalidOperationException(),
                    StartsAtUtc = @event.StartsAtUtc,
                    EndsAtUtc = @event.EndsAtUtc ?? throw new InvalidOperationException(),
                    Body = @event.Body,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Status = status,
                    Visibility = @event.Visibility,
                    CreatedAtUtc = utcNow,
                    UpdatedAtUtc = utcNow,
                    PublishedAtUtc = @event.PublishedAtUtc,
                    TimeZoneId = "Pacific/Auckland"
                };

                context.Add(eventOccurrence);

                return 1;
            }
            else
            {
                var eventOccurrenceCount = 0;
                var recurrenceRule = new RecurrenceRule(@event.RecurrenceRule);

                var occurrencesNz = recurrenceRule.GetOccurrence(@event.TimeZoneId, @event.StartsAtUtc.ConvertUtcToNz(), @event.EndsAtUtc?.ConvertUtcToNz(), @event.Duration);

                foreach (var occurrenceNz in occurrencesNz)
                {
                    var eventOccurrence = new EventOccurrence
                    {
                        Id = Guid.CreateVersion7(),
                        Handle = occurrenceNz.Item1.GetDate(),
                        Type = ContentType.EventOccurrence,
                        ParentId = @event.Id,
                        UserId = @event.UserId,
                        SpaceId = @event.SpaceId,
                        Title = @event.Title ?? throw new InvalidOperationException(),
                        StartsAtUtc = occurrenceNz.Item1.ConvertNzToUtc(),
                        EndsAtUtc = occurrenceNz.Item2.ConvertNzToUtc(),
                        Body = @event.Body,
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Status = status,
                        CreatedAtUtc = utcNow,
                        UpdatedAtUtc = utcNow,
                        Visibility = @event.Visibility,
                        TimeZoneId = @event.TimeZoneId
                    };

                    context.Add(eventOccurrence);
                    eventOccurrenceCount++;
                }

                return eventOccurrenceCount;
            }
        }
    }
}
