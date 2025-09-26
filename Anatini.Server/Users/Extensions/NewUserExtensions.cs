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
                Guid = userEmail.Guid,
                UserGuid = newUser.Guid,
                Address = userEmail.Address,
                Verified = true
            };

            var userOwnedSession = new UserOwnedSession
            {
                Guid = Guid.NewGuid(),
                UserGuid = newUser.Guid,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var userOwnedSlug = new UserOwnedAlias
            {
                Guid = newUser.SlugId,
                UserGuid = newUser.Guid,
                Slug = newUser.Slug
            };

            var user = new User
            {
                Guid = newUser.Guid,
                Name = newUser.Name,
                HashedPassword = null!,
                Emails = [userOwnedEmail],
                Sessions = [userOwnedSession],
                Aliases = [userOwnedSlug],
                DefaultAliasGuid = newUser.SlugId
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, newUser.Password);

            return user;
        }

        public static UserAlias CreateSlug(this NewUser newUser)
        {
            return new UserAlias
            {
                Guid = newUser.SlugId,
                Slug = newUser.Slug,
                UserGuid = newUser.Guid,
                UserName = newUser.Name
            };
        }
    }
}
