using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEmailContextExtensions
    {
        public static async Task<int> AddUserEmailAsync(this AnatiniContext context, string address, string userId)
        {
            var userEmail = new UserEmail
            {
                ItemId = ItemId.Get(address),
                Address = address,
                UserId = userId,
                VerificationCode = RandomHex.NextX8(),
                Verified = false
            };

            return await context.AddAsync(userEmail);
        }
    }
}
