using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class EventContextExtensions
    {
        public static EventSeries AddUserEventSeries(this ApplicationDbContext context, Visibility visibility, Guid userId, string name, DateTime startsAtNZ)
        {
            var eventSeries = new EventSeries
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Visibility = visibility,
                Name = name,
                Duration = new TimeSpan(1, 0, 0),
                StartsAtUtc = startsAtNZ.ConvertNzToUtc(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            context.Add(eventSeries);

            return eventSeries;
        }

        public static int AddEventInstances(this ApplicationDbContext context, EventSeries eventSeries)
        {
            if (eventSeries.RecurrenceRule == null)
            {
                var eventInstance = new EventInstance
                {
                    Id = Guid.CreateVersion7(),
                    EventSeriesId = eventSeries.Id,
                    UserId = eventSeries.UserId,
                    SpaceId = eventSeries.SpaceId,
                    Name = eventSeries.Name,
                    StartsAtUtc = eventSeries.StartsAtUtc,
                    EndsAtUtc = eventSeries.StartsAtUtc.Add(eventSeries.Duration)
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
