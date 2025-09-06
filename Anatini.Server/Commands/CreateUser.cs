using Anatini.Server.Authentication;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Commands
{
    internal class CreateUser(string name, string handleValue, string password, Email email, Guid userId, Guid handleId, string refreshToken, EventData eventData) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new UserEmail
            {
                EmailId = email.Id,
                Value = email.Value,
                Verified = true
            };

            var userSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                RefreshToken = refreshToken,
                CreatedDateUtc = eventData.DateTimeUtc,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var userHandle = new UserHandle
            {
                HandleId = handleId,
                Value = handleValue
            };

            var user = new User
            {
                Id = userId,
                Name = name,
                HashedPassword = null!,
                Emails = [userEmail],
                CreatedDateUTC = eventData.DateTimeUtc,
                Sessions = [userSession],
                Handles = [userHandle],
                DefaultHandleId = handleId
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
