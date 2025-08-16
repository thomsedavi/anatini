using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateEmailUser(string email, Guid userId, DateOnly createdDate) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.EmailUsers.Add(new EmailUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserId = userId,
                VerificationCode = CodeRandom.Next(),
                CreatedDate = createdDate
            });

            return await context.SaveChangesAsync();
        }
    }
}
