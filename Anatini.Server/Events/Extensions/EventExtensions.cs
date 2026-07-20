using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Events.Extensions
{
    public static class EventExtensions
    {
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
