using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    internal class GetUser(Guid userId) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Users.WithPartitionKey(userId).FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}
