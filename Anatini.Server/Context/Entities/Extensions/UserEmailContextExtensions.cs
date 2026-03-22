using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserEmailContextExtensions
    {
        public static ApplicationUserEmail AddUserEmailAsync(this ApplicationDbContext context, string address)
        {
            var userEmail = new ApplicationUserEmail
            {
                Email = address,
                NormalizedEmail = address.ToUpperInvariant(),
                ConfirmationCode = RandomHex.NextX8()
            };

            context.Add(userEmail);

            return userEmail;
        }
    }
}
