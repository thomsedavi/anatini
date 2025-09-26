using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    public class GetUser(Guid userGuid) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Users.WithPartitionKey(userGuid).FirstOrDefaultAsync(user => user.Guid == userGuid);
        }
    }
}
