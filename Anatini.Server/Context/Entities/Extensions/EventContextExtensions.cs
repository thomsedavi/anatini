using Anatini.Server.Enums;
using Anatini.Server.Events;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class EventContextExtensions
    {
        public static EventSeries AddUserEventSeries(this ApplicationDbContext context, Guid userId, CreateEvent createEvent)
        {
            var eventSeriesId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var eventSeries = new EventSeries
            {
                Id = eventSeriesId,
                UserId = userId,
                Visibility = createEvent.Visibility,
                Name = createEvent.Name,
                Article = createEvent.Article,
                Url = createEvent.Url,
                StartsAtNz = createEvent.StartsAtNz,
                EndsAtNz = createEvent.EndsAtNz,
                Duration = createEvent.Duration,
                RecurrenceRule = createEvent.RecurrenceRule,
                ExpiresAtNz = createEvent.StartsAtNz,
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

                // Todo

                return eventInstanceCount;
            }
        }
    }
}
