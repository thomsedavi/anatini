using Anatini.Server.Context;
using Anatini.Server.Enums;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Commands
{
    internal class CreateUserEvent(Guid userId, EventType eventType, EventData data) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEvent = new UserEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventType = Enum.GetName(eventType)!,
                DateTimeUtc = data.DateTimeUtc,
                Data = data.ToDictionary()
            };

            context.Add(userEvent);

            return await context.SaveChangesAsync();
        }
    }
}
