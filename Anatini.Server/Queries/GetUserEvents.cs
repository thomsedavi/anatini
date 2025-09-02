using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    public class GetUserEvents(Guid userId) : IQuery<IEnumerable<UserEvent>>
    {
        public async Task<IEnumerable<UserEvent>> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserEvents.Where(user => user.UserId == userId).ToListAsync();
        }
    }
}
