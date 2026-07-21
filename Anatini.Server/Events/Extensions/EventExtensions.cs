using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Events.Extensions
{
    public static class EventExtensions
    {
        public static EventOccurrenceDto ToEventSeriesDto(this EventSeries eventSeries)
        {
            return new EventOccurrenceDto
            {
                Id = eventSeries.Id,
                Name = eventSeries.Name
            };
        }

        public static EventDto ToEventInstanceDto(this EventInstance eventInstance)
        {
            return new EventDto
            {
                Id = eventInstance.Id,
                Handle = eventInstance.Handle,
                StartsAtNz = eventInstance.StartsAtNz,
                EndsAtNz = eventInstance.EndsAtNz
            };
        }
    }
}
