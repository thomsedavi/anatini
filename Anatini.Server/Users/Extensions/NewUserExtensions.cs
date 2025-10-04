using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserExtensions
    {
        public static User Create(this NewUser newUser, UserEmail userEmail, string refreshToken, EventData eventData)
        {
            var userOwnedEmail = new UserOwnedEmail
            {
                Address = userEmail.Address,
                UserId = newUser.Id,
                Verified = true
            };

            var userOwnedSession = new UserOwnedSession
            {
                Id = IdGenerator.Get(),
                UserId = newUser.Id,
                RefreshToken = refreshToken,
                CreatedDateTimeUtc = eventData.DateTimeUtc,
                UpdatedDateTimeUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("ipAddress"),
                UserAgent = eventData.Get("userAgent"),
                Revoked = false
            };

            var userOwnedSlug = new UserOwnedAlias
            {
                Slug = newUser.Slug,
                UserId = newUser.Id
            };

            var user = new User
            {
                Id = newUser.Id,
                Name = newUser.Name,
                HashedPassword = null!,
                Emails = [userOwnedEmail],
                Sessions = [userOwnedSession],
                Aliases = [userOwnedSlug],
                DefaultSlug = newUser.Slug
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, newUser.Password);

            return user;
        }

        public static UserAlias CreateSlug(this NewUser newUser)
        {
            return new UserAlias
            {
                Id = newUser.Slug,
                Slug = newUser.Slug,
                UserId = newUser.Id,
                UserName = newUser.Name
            };
        }
    }
}
