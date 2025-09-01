using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    internal class GetEmailUser(string email) : IQuery<EmailUser?>
    {
        public async Task<EmailUser?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.EmailUsers.FirstOrDefaultAsync(emailUser => emailUser.Email == email);
        }
    }
}
