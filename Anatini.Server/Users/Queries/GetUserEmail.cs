using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users.Queries
{
    internal class GetUserEmail(string emailAddress) : IQuery<UserEmail?>
    {
        public async Task<UserEmail?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserEmails.WithPartitionKey(emailAddress).FirstOrDefaultAsync(email => email.Address == emailAddress);
        }
    }
}
