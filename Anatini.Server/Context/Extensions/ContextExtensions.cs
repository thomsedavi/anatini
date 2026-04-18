using Anatini.Server.Context.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Extensions
{
    public static class ContextExtensions
    {
        public static async Task<ApplicationUser?> GetUserAsync(this ApplicationDbContext context, string normalizedEmail)
        {
            return await context.Users.FirstOrDefaultAsync(user => user.Emails.Any(userEmail => userEmail.NormalizedEmail.Equals(normalizedEmail)));
        }
    }
}
