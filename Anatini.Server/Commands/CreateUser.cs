using Anatini.Server.Authentication;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Commands
{
    internal class CreateUser(string name, string password, EmailUser email, Guid userId, string refreshToken, EventData eventData) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new UserEmail
            {
                Id = email.Id,
                Email = email.Email,
                Verified = true
            };

            var userRefreshToken = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                RefreshToken = refreshToken,
                CreatedDateNZ = eventData.DateOnlyNZNow,
                IPAddress = eventData.Get("IPAddress"),
                UserAgent = eventData.Get("UserAgent"),
                Revoked = false
            };

            var user = new User
            {
                Id = userId,
                Name = name,
                HashedPassword = null!,
                Emails = [userEmail],
                CreatedDateNZ = eventData.DateOnlyNZNow,
                RefreshTokens = [userRefreshToken]
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Users.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
