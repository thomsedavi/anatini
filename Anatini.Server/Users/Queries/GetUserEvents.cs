using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    public class GetUserEvents(Guid userId) : IQuery<IEnumerable<UserEvent>>
    {
        public async Task<IEnumerable<UserEvent>> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserEvents.WithPartitionKey(userId).Where(userEvent => userEvent.UserId == userId).ToListAsync();
        }
    }
}
