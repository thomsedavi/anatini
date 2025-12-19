using Anatini.Server.Authentication;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserContextExtensions
    {
        public static async Task<User> AddUserAsync(this AnatiniContext context, Guid id, string name, string slug, string password, string address, bool? @protected, string refreshToken, EventData eventData)
        {
            var userOwnedAlias = new UserOwnedAlias
            {
                UserId = id,
                Slug = slug
            };

            var userOwnedEmail = new UserOwnedEmail {
                UserId= id,
                Address = address,
                Verified = true
            };

            var userOwnedSession = new UserOwnedSession
            {
                Id = Guid.NewGuid(),
                UserId = id,
                RefreshToken = refreshToken,
                CreatedDateTimeUtc = eventData.DateTimeUtc,
                UpdatedDateTimeUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("ipAddress"),
                UserAgent = eventData.Get("userAgent"),
                Revoked = false
            };

            var user = new User
            {
                ItemId = ItemId.Get(id),
                Id = id,
                Name = name,
                DefaultSlug = slug,
                Aliases = [userOwnedAlias],
                HashedPassword = null!,
                Emails = [userOwnedEmail],
                Sessions = [userOwnedSession],
                Protected = @protected
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            await context.Add(user);

            return user;
        }

        public static async Task<User?> VerifyPassword(this AnatiniContext context, string address, string password)
        {
            var userId = (await context.FindAsync<UserEmail>(address))?.UserId;

            if (!userId.HasValue)
            {
                return null;
            }

            var user = await context.FindAsync<User>(userId);

            if (user == null)
            {
                return null;
            }

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
