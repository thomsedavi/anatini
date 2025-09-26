using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class GetUserInvite(string inviteCode) : IQuery<UserInvite?>
    {
        public async Task<UserInvite?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserInvites.WithPartitionKey(inviteCode).FirstOrDefaultAsync(invite => invite.Code == inviteCode);
        }
    }
}
