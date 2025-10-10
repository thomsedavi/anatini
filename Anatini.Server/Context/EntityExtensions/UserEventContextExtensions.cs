using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserEventContextExtensions
    {
        public static async Task<UserEvent> AddUserEventAsync(this AnatiniContext context, Guid userId, EventType eventType, EventData data)
        {
            var userEvent = new UserEvent
            {
                ItemId = ItemId.Get(Guid.NewGuid()),
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
