using Anatini.Server.Authentication;
using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserExtensions
    {
        public static User Create(this NewUser newUser, NewUserSlug newUserSlug, Email email, string refreshToken, EventData eventData)
        {
            var userEmail = new UserOwnedEmail
            {
                EmailId = email.Id,
                UserId = newUser.Id,
                Address = email.Address,
                Verified = true
            };

            var userSession = new UserOwnedSession
            {
                SessionId = Guid.NewGuid(),
                UserId = newUser.Id,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var userSlug = new UserOwnedSlug
            {
                SlugId = newUserSlug.Id,
                UserId = newUser.Id,
                Slug = newUser.Slug
            };

            var user = new User
            {
                Id = newUser.Id,
                Name = newUser.Name,
                HashedPassword = null!,
                Emails = [userEmail],
                Sessions = [userSession],
                Slugs = [userSlug],
                DefaultSlugId = newUserSlug.Id
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, newUser.Password);

            return user;
        }
    }
}
