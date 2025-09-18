using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateInvite(Guid id, string code, Guid userId, DateOnly dateNZ) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var invite = new Invite
            {
                Id = id,
                Code = code,
                InvitedByUserId = userId,
                NewUserId = Guid.NewGuid(),
                DateNZ = dateNZ
            };

            context.Add(invite);

            return await context.SaveChangesAsync();
        }
    }
}
