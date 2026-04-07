using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEmailContextExtensions
    {
        public static ApplicationUserEmail AddUserEmail(this ApplicationDbContext context, string email, string normalizedEmail)
        {
            var userEmail = new ApplicationUserEmail
            {
                Email = email,
                NormalizedEmail = normalizedEmail,
                ConfirmationCode = RandomHex.NextX8()
            };

            context.Add(userEmail);

            return userEmail;
        }
    }
}
