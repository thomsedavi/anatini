using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Channels.Queries
{
    public class GetChannelAlias(string slug) : IQuery<ChannelAlias?>
    {
        public async Task<ChannelAlias?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.ChannelAliases.FindAsync(slug);
        }
    }
}
