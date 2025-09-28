using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateUserInvite(string code, Guid userId, DateOnly dateOnlyNZ) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userInvite = new UserInvite
            {
                Code = code,
                InvitedByUserId = userId,
                NewUserId = Guid.NewGuid(),
                DateOnlyNZ = dateOnlyNZ
            };

            context.Add(userInvite);

            return await context.SaveChangesAsync();
        }
    }
}
