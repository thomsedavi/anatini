using Anatini.Server.Context;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserEventExtensions
    {
        public static AnatiniContext AddUserEvent(this AnatiniContext context, string userId, EventType eventType, EventData data)
        {
            var userEvent = new UserEvent
            {
                Id = IdGenerator.Get(),
                UserId = userId,
                EventType = Enum.GetName(eventType)!,
                DateTimeUtc = data.DateTimeUtc,
                Data = data.ToDictionary()
            };

            context.Add(userEvent);

            return context;
        }

        public static UserEventDto ToUserEventDto(this UserEvent userEvent)
        {
            return new UserEventDto
            {
                EventType = userEvent.EventType,
                DateTimeUtc = userEvent.DateTimeUtc
            };
        }
    }
}
