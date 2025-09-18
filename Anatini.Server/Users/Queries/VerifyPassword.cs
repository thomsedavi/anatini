using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class VerifyPassword(string emailAddress, string password) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userId = await context.Emails
                .WithPartitionKey(emailAddress)
                .Where(email => email.Address == emailAddress)
                .Select(email => email.UserId)
                .FirstOrDefaultAsync();

            if (userId == Guid.Empty)
            {
                return null;
            }

            var userResult = await context.Users
                .WithPartitionKey(userId)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (userResult == null)
            {
                return null;
            }

            var user = userResult!;

            var result = UserPasswordHasher.VerifyHashedPassword(user, password);

            if (result == PasswordVerificationResult.Success)
            {
                return userResult;
            }
            else
            {
                return null;
            }
        }
    }
}
