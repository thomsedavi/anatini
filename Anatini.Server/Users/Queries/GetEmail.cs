using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class GetEmail(string emailAddress) : IQuery<Email?>
    {
        public async Task<Email?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Emails.WithPartitionKey(emailAddress).FirstOrDefaultAsync(email => email.Address == emailAddress);
        }
    }
}
