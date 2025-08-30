using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Queries
{
    public class GetUser(Guid userId) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Users.FindAsync(userId);
        }
    }
}
