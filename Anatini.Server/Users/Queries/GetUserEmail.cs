using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Queries
{
    internal class GetUserEmail(string address) : IQuery<UserEmail?>
    {
        public async Task<UserEmail?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.UserEmails.FindAsync(address);
        }
    }
}
