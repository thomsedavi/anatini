using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserInviteContextExtensions
    {
        public static async Task<UserInvite> AddUserInviteAsync(this AnatiniContext context, string code, string userId, DateOnly dateOnlyNZ)
        {
            var userInvite = new UserInvite
            {
                ItemId = code,
                Code = code,
                UserId = userId,
                NewUserId = IdGenerator.Get(),
                DateOnlyNZ = dateOnlyNZ
            };

            await context.Add(userInvite);

            return userInvite;
        }
    }
}
