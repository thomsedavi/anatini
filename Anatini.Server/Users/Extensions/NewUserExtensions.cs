using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserExtensions
    {
        public static User Create(this NewUser newUser, Email email, string refreshToken, EventData eventData)
        {
            var userOwnedEmail = new UserOwnedEmail
            {
                Id = email.Id,
                UserId = newUser.Id,
                Address = email.Address,
                Verified = true
            };

            var userOwnedSession = new UserOwnedSession
            {
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var userOwnedSlug = new UserOwnedSlug
            {
                Id = newUser.SlugId,
                UserId = newUser.Id,
                Slug = newUser.Slug
            };

            var user = new User
            {
                Id = newUser.Id,
                Name = newUser.Name,
                HashedPassword = null!,
                Emails = [userOwnedEmail],
                Sessions = [userOwnedSession],
                Slugs = [userOwnedSlug],
                DefaultSlugId = newUser.SlugId
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, newUser.Password);

            return user;
        }

        public static UserSlug CreateSlug(this NewUser newUser)
        {
            return new UserSlug
            {
                Id = newUser.SlugId,
                Slug = newUser.Slug,
                UserId = newUser.Id,
                UserName = newUser.Name
            };
        }
    }
}
