using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Commands
{
    internal class CreateEmail(string address, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var email = new Email
            {
                Id = Guid.NewGuid(),
                Address = address,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            context.Add(email);

            return await context.SaveChangesAsync();
        }
    }
}
