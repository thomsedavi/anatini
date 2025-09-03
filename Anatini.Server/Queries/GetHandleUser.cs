using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    public class GetHandleUser(string handle) : IQuery<HandleUser?>
    {
        public async Task<HandleUser?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.HandleUsers.FirstOrDefaultAsync(handleUser => handleUser.Handle == handle);
        }
    }
}
