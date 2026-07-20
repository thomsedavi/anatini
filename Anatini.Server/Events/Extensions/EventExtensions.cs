using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Events.Extensions
{
    public static class EventExtensions
    {
        public static EventSeriesDto ToEventSeriesDto(this EventSeries eventSeries)
        {
            return new EventSeriesDto
            {
                Id = eventSeries.Id,
                Name = eventSeries.Name
            };
        }

        public static EventInstanceDto ToEventInstanceDto(this EventInstance eventInstance)
        {
            return new EventInstanceDto
            {
                Id = eventInstance.Id,
                Handle = eventInstance.Handle,
                StartsAtNz = eventInstance.StartsAtNz,
                EndsAtNz = eventInstance.EndsAtNz
            };
        }
    }
}
