using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Commands
{
    internal class CreateUserEmail(string address, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new UserEmail
            {
                Guid = Guid.NewGuid(),
                Address = address,
                UserGuid = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            context.Add(userEmail);

            return await context.SaveChangesAsync();
        }
    }
}
