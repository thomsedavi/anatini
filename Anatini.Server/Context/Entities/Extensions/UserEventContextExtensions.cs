using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEventContextExtensions
    {
        public static async Task<UserEvent> AddUserEventAsync(this AnatiniContext context, string userId, EventType eventType, EventData data)
        {
            var userEvent = new UserEvent
            {
                ItemId = ItemId.Get(RandomHex.NextX16()),
                UserId = userId,
                EventType = Enum.GetName(eventType)!,
                DateTimeUtc = data.DateTimeUtc,
                Data = data.ToDictionary()
            };

            await context.AddAsync(userEvent);

            return userEvent;
        }
    }
}
