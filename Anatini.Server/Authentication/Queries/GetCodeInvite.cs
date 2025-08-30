using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Authentication.Queries
{
    internal class GetCodeInvite(string code) : IQuery<CodeInvite?>
    {
        public async Task<CodeInvite?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.CodeInvites.FirstOrDefaultAsync(invite => invite.InviteCode == code);
        }
    }
}
