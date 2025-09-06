using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    public class GetEvents(Guid userId) : IQuery<IEnumerable<Event>>
    {
        public async Task<IEnumerable<Event>> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Events.WithPartitionKey(userId).Where(user => user.UserId == userId).ToListAsync();
        }
    }
}
