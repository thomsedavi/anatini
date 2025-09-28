using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Channels.Queries
{
    public class GetChannel(Guid id) : IQuery<Channel?>
    {
        public async Task<Channel?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Channels.FindAsync(id);
        }
    }
}
