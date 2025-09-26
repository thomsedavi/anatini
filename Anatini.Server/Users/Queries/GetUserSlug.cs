using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    public class GetUserSlug(string slug) : IQuery<UserAlias?>
    {
        public async Task<UserAlias?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserAliases.WithPartitionKey(slug).FirstOrDefaultAsync(userAlias => userAlias.Slug == slug);
        }
    }
}
