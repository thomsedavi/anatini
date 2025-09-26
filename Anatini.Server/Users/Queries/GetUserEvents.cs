using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    public class GetUserEvents(Guid userGuid) : IQuery<IEnumerable<UserEvent>>
    {
        public async Task<IEnumerable<UserEvent>> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserEvents.WithPartitionKey(userGuid).Where(user => user.UserGuid == userGuid).ToListAsync();
        }
    }
}
