using Anatini.Server.Context.Entities;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Authentication
{
    public static class UserPasswordHasher
    {
        private static readonly PasswordHasher<User> passwordHasher = new();

        internal static string HashPassword(User user, string password)
        {
            return passwordHasher.HashPassword(user, password);
        }

        internal static PasswordVerificationResult VerifyHashedPassword(User user, string password)
        {
            return passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
        }
    }
}
