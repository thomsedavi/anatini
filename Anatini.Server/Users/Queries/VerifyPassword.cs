using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class VerifyPassword(string address, string password) : IQuery<User?>
    {
        public async Task<User?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userGuid = await context.UserEmails
                .WithPartitionKey(address)
                .Where(email => email.Address == address)
                .Select(email => email.UserGuid)
                .FirstOrDefaultAsync();

            if (userGuid == Guid.Empty)
            {
                return null;
            }

            var userResult = await context.Users
                .WithPartitionKey(userGuid)
                .FirstOrDefaultAsync(user => user.Guid == userGuid);

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
