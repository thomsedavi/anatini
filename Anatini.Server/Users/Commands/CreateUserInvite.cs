using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateUserInvite(Guid guid, string code, Guid userGuid, DateOnly dateNZ) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userInvite = new UserInvite
            {
                Guid = guid,
                Code = code,
                InvitedByUserGuid = userGuid,
                NewUserGuid = Guid.NewGuid(),
                DateNZ = dateNZ
            };

            context.Add(userInvite);

            return await context.SaveChangesAsync();
        }
    }
}
