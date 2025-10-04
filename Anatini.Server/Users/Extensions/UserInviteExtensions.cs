using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserInviteExtensions
    {
        public static AnatiniContext AddUserInvite(this AnatiniContext context, string code, string userId, DateOnly dateOnlyNZ)
        {
            var userInvite = new UserInvite
            {
                Id = code,
                Code = code,
                UserId = userId,
                NewUserId = IdGenerator.Get(),
                DateOnlyNZ = dateOnlyNZ
            };

            context.Add(userInvite);

            return context;
        }
    }
}
