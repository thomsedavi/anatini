using Anatini.Server.Enums;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Commands
{
    internal class CreateEvent(Guid userId, EventType type, EventData data) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var @event = new Event
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = Enum.GetName(type)!,
                CreatedDateUtc = data.DateTimeUtc,
                Data = data.ToDictionary()
            };

            context.Add(@event);

            return await context.SaveChangesAsync();
        }
    }
}