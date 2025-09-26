using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Channels.Queries
{
    public class GetChannelAlias(string slug) : IQuery<ChannelAlias?>
    {
        public async Task<ChannelAlias?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.ChannelAliases.WithPartitionKey(slug).FirstOrDefaultAsync(channelSlug => channelSlug.Slug == slug);
        }
    }
}
