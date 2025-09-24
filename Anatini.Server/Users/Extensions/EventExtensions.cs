using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Users.Extensions
{
    public static class EventExtensions
    {
        public static EventDto ToEventDto(this Event @event)
        {
            return new EventDto
            {
                Type = @event.Type,
                DateUtc = @event.DateUtc
            };
        }
    }
}
