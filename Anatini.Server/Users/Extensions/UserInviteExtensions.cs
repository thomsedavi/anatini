using Anatini.Server.Context;

namespace Anatini.Server.Users.Extensions
{
    public static class UserInviteExtensions
    {
        public static AnatiniContext AddUserInvite(this AnatiniContext context, string code, Guid userId, DateOnly dateOnlyNZ)
        {
            var userInvite = new UserInvite
            {
                Code = code,
                InvitedByUserId = userId,
                NewUserId = Guid.NewGuid(),
                DateOnlyNZ = dateOnlyNZ
            };

            context.Add(userInvite);

            return context;
        }
    }
}
