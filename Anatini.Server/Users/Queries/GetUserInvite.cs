using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Queries
{
    internal class GetUserInvite(string code) : IQuery<UserInvite?>
    {
        public async Task<UserInvite?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserInvites.FindAsync(code);
        }
    }
}
