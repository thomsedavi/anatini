using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateEmailUser(string email, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new EmailUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            context.EmailUsers.Add(userEmail);

            return await context.SaveChangesAsync();
        }
    }
}
