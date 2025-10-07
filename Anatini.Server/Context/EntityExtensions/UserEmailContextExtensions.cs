using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserEmailContextExtensions
    {
        public static async Task<int> AddUserEmailAsync(this AnatiniContext context, string address, string userId)
        {
            var userEmail = new UserEmail
            {
                ItemId = address,
                Address = address,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            return await context.Add(userEmail);
        }
    }
}
