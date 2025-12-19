using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEmailContextExtensions
    {
        public static async Task<int> AddUserEmailAsync(this AnatiniContext context, string address, Guid userId)
        {
            var userEmail = new UserEmail
            {
                ItemId = ItemId.Get(address),
                Address = address,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            return await context.Add(userEmail);
        }
    }
}
