using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class GetInvite(string inviteCode) : IQuery<Invite?>
    {
        public async Task<Invite?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Invites.WithPartitionKey(inviteCode).FirstOrDefaultAsync(invite => invite.Code == inviteCode);
        }
    }
}
