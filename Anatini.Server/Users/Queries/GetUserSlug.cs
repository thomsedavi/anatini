using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    public class GetUserSlug(string slug) : IQuery<UserSlug?>
    {
        public async Task<UserSlug?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserSlugs.WithPartitionKey(slug).FirstOrDefaultAsync(userSlug => userSlug.Slug == slug);
        }
    }
}
