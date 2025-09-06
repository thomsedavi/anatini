using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    internal class GetEmail(string emailValue) : IQuery<Email?>
    {
        public async Task<Email?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Emails.WithPartitionKey(emailValue).FirstOrDefaultAsync(email => email.Value == emailValue);
        }
    }
}
