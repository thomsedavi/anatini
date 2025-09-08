using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateInvite(string inviteValue, Guid userId, Guid inviteId, DateOnly dateNZ) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var invite = new Invite
            {
                Id = inviteId,
                Value = inviteValue,
                InvitedByUserId = userId,
                NewUserId = Guid.NewGuid(),
                DateNZ = dateNZ
            };

            context.Add(invite);

            return await context.SaveChangesAsync();
        }
    }
}
