using Anatini.Server.Interfaces;
using Anatini.Server.Utils;

namespace Anatini.Server.Commands
{
    internal class CreateEmail(string emailValue, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var email = new Email
            {
                Id = Guid.NewGuid(),
                Value = emailValue,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                Verified = false
            };

            context.Add(email);

            return await context.SaveChangesAsync();
        }
    }
}
