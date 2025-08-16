using Anatini.Server.Enums;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateUserEvent(Guid userId, UserEventType type, IDictionary<string, string>? details = null) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEvent = new UserEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = Enum.GetName(type)!,
                DateTimeUtc = DateTime.UtcNow,
                Details = details
            };

            context.UserEvents.Add(userEvent);

            return await context.SaveChangesAsync();
        }
    }
}