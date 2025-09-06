using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    internal class GetInvite(string inviteValue) : IQuery<Invite?>
    {
        public async Task<Invite?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Invites.WithPartitionKey(inviteValue).FirstOrDefaultAsync(invite => invite.Value == inviteValue);
        }
    }
}
