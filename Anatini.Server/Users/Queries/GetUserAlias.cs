using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Queries
{
    public class GetUserAlias(string slug) : IQuery<UserAlias?>
    {
        public async Task<UserAlias?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserAliases.FindAsync(slug);
        }
    }
}
