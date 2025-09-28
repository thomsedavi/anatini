using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Users.Queries
{
    internal class VerifyPassword(string address, string password) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userId = (await context.UserEmails.FindAsync(address))?.UserId;

            if (!userId.HasValue)
            {
                return null;
            }

            var userResult = await context.Users.FindAsync(userId.Value);

            if (userResult == null)
            {
                return null;
            }

            var user = userResult!;

            var result = UserPasswordHasher.VerifyHashedPassword(user, password);

            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
