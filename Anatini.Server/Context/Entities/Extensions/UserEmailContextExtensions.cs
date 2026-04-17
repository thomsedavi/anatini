using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEmailContextExtensions
    {
        public static ApplicationUserEmail AddUserEmail(this ApplicationDbContext context, string email, string normalizedEmail)
        {
            var utcNow = DateTime.UtcNow;

            var userEmail = new ApplicationUserEmail
            {
                Id = Guid.CreateVersion7(),
                Email = email,
                NormalizedEmail = normalizedEmail,
                ConfirmationCode = RandomHex.NextX8(),
                EmailConfirmed = false,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(userEmail);

            return userEmail;
        }
    }
}
