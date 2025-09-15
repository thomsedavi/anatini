using Anatini.Server.Authentication;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Commands
{
    internal class CreateUser(Guid id, string name, string slug, string password, Email email, Guid slugId, string refreshToken, EventData eventData) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new UserOwnedEmail
            {
                EmailId = email.Id,
                UserId = id,
                Address = email.Address,
                Verified = true
            };

            var userSession = new UserOwnedSession
            {
                SessionId = Guid.NewGuid(),
                UserId = id,
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                UpdatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var userSlug = new UserOwnedSlug
            {
                SlugId = slugId,
                UserId = id,
                Slug = slug
            };

            var user = new User
            {
                Id = id,
                Name = name,
                HashedPassword = null!,
                Emails = [userEmail],
                Sessions = [userSession],
                Slugs = [userSlug],
                DefaultSlugId = slugId
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
