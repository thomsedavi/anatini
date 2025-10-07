using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserEventContextExtensions
    {
        public static async Task<UserEvent> AddUserEventAsync(this AnatiniContext context, string userId, EventType eventType, EventData data)
        {
            var userEvent = new UserEvent
            {
                ItemId = IdGenerator.Get(),
                UserId = userId,
                EventType = Enum.GetName(eventType)!,
                DateTimeUtc = data.DateTimeUtc,
                Data = data.ToDictionary()
            };

            await context.Add(userEvent);

            return userEvent;
        }
    }
}
