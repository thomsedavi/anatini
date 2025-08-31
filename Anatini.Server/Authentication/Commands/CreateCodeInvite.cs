using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    public class CreateCodeInvite(string inviteCode, DateOnly dateOnlyNZ, Guid userId, Guid inviteId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var codeInvite = new CodeInvite
            {
                Id = inviteId,
                InviteCode = inviteCode,
                InvitedByUserId = userId,
                NewUserId = Guid.NewGuid(),
                CreatedDateNZ = dateOnlyNZ
            };

            context.CodeInvites.Add(codeInvite);

            return await context.SaveChangesAsync();
        }
    }
}
