using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserInviteContextExtensions
    {
        public static async Task<UserInvite> AddUserInviteAsync(this AnatiniContext context, string code, Guid userId, DateOnly dateOnlyNZ)
        {
            var userInvite = new UserInvite
            {
                ItemId = ItemId.Get(code),
                Code = code,
                UserId = userId,
                NewUserId = Guid.NewGuid(),
                DateOnlyNZ = dateOnlyNZ
            };

            await context.Add(userInvite);

            return userInvite;
        }
    }
}
