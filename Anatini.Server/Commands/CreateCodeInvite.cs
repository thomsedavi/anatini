using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateCodeInvite(string inviteCode, Guid userId, Guid inviteId, DateOnly createdDateNZ) : ICommand<int>
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
                CreatedDateNZ = createdDateNZ
            };

            context.CodeInvites.Add(codeInvite);

            return await context.SaveChangesAsync();
        }
    }
}
