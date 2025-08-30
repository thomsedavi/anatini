using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class CreateUser(string name, string password, EmailUser email, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            var dateTimeNZ = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfoNZ);
            var createdDate = DateOnly.FromDateTime(dateTimeNZ);

            var userEmail = new UserEmail
            {
                Id = email.Id,
                Email = email.Email,
                Verified = true
            };

            var user = new User
            {
                Id = userId,
                Name = name,
                HashedPassword = null!,
                Emails = [userEmail],
                Invites= [],
                CreatedDate = createdDate
            };

            user.HashedPassword = UserPasswordHasher.HashPassword(user, password);

            context.Users.Add(user);

            return await context.SaveChangesAsync();
        }
    }
}
