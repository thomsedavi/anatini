using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserEmailExtensions
    {
        public static AnatiniContext AddUserEmail(this AnatiniContext context, string address, Guid userId)
        {
            var userEmail = new UserEmail
            {
                Address = address,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            context.Add(userEmail);

            return context;
        }
    }
}
