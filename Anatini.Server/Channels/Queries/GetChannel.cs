using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Channels.Queries
{
    public class GetChannel(Guid guid) : IQuery<Channel?>
    {
        public async Task<Channel?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Channels.WithPartitionKey(guid).FirstOrDefaultAsync(channel => channel.Guid == guid);
        }
    }
}
