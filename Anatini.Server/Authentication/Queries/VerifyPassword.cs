using Anatini.Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Authentication.Queries
{
    internal class VerifyPassword(string email, string password) : IQuery<Guid?>
    {
        public async Task<Guid?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userId = await context.EmailUsers
                .WithPartitionKey(email)
                .Where(emailUser => emailUser.Email == email)
                .Select(emailUser => emailUser.UserId)
                .FirstAsync();

            var user = await context.Users
                .WithPartitionKey(userId)
                .FirstAsync(user => user.Id == userId);

            var result = UserPasswordHasher.VerifyHashedPassword(user, password);

            if (result == PasswordVerificationResult.Success)
            {
                return userId;
            }
            else
            {
                return null;
            }
        }
    }
}
