using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateUser(string name, string password, string email, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userEmail = new UserEmail { Email = email, IsVerified = false };

            var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            var dateTimeNZ = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfoNZ);
            var createdDate = DateOnly.FromDateTime(dateTimeNZ);

            var user = new User
            {
                Id = userId,
                Name = name,
                HashedPassword = null!,
                Emails = [userEmail],
                CreatedDate = createdDate
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Users.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
