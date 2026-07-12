using Anatini.Server.Enums;
using Anatini.Server.Events;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class EventContextExtensions
    {
        public static EventSeries AddUserEventSeries(this ApplicationDbContext context, Guid userId, CreateEvent createEvent, Status status)
        {
            var eventSeriesId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var recurrenceRule = new RecurrenceRule(createEvent.RecurrenceRule);

            var eventSeries = new EventSeries
            {
                Id = eventSeriesId,
                UserId = userId,
                Status = status,
                Visibility = createEvent.Visibility,
                Name = createEvent.Name,
                Article = createEvent.Article,
                Url = createEvent.Url,
                StartsAtNz = createEvent.StartsAtNz,
                EndsAtNz = createEvent.EndsAtNz,
                Duration = createEvent.Duration,
                RecurrenceRule = createEvent.RecurrenceRule,
                ExpiresAtNz = recurrenceRule.ExpiresAtNz(createEvent.StartsAtNz, createEvent.EndsAtNz, createEvent.Duration),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                Handle = createEvent.Handle ?? eventSeriesId.ToString()
            };

            context.Add(eventSeries);

            return eventSeries;
        }

        public static int AddEventInstances(this ApplicationDbContext context, EventSeries eventSeries, Status status)
        {
            if (eventSeries.RecurrenceRule == null)
            {
                var eventInstance = new EventInstance
                {
                    Id = Guid.CreateVersion7(),
                    Handle = eventSeries.StartsAtNz.GetDate(),
                    EventSeriesId = eventSeries.Id,
                    UserId = eventSeries.UserId,
                    SpaceId = eventSeries.SpaceId,
                    Name = eventSeries.Name,
                    StartsAtNz = eventSeries.StartsAtNz,
                    EndsAtNz = eventSeries.StartsAtNz,
                    Article = eventSeries.Article,
                    Url = eventSeries.Url,
                    Status = status,
                    Visibility = eventSeries.Visibility
                };

                context.Add(eventInstance);
                return 1;
            }
            else
            {
                var eventInstanceCount = 0;
                var recurrenceRule = new RecurrenceRule(eventSeries.RecurrenceRule);

                var instances = recurrenceRule.GetInstances(eventSeries.StartsAtNz, eventSeries.EndsAtNz, eventSeries.Duration);

                foreach (var instance in instances)
                {
                    var eventInstance = new EventInstance
                    {
                        Id = Guid.CreateVersion7(),
                        Handle = instance.Item1.GetDate(),
                        EventSeriesId = eventSeries.Id,
                        UserId = eventSeries.UserId,
                        SpaceId = eventSeries.SpaceId,
                        Name = eventSeries.Name,
                        StartsAtNz = instance.Item1,
                        EndsAtNz = instance.Item2,
                        Article = eventSeries.Article,
                        Url = eventSeries.Url,
                        Status = status,
                        Visibility = eventSeries.Visibility
                    };

                    context.Add(eventInstance);
                    eventInstanceCount++;
                }

                return eventInstanceCount;
            }
        }
    }
}
