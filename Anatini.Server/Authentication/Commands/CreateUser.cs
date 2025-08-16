using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateUser(string name, string password, string email, Guid userId, DateOnly createdDate) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var user = new User
            {
                Id = userId,
                Name = name,
                HashedPassword = null!,
                Emails = [new UserEmail{ Email = email, IsVerified = false }],
                CreatedDate = createdDate
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Users.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
