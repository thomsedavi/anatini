using Anatini.Server.Context;
using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Channels.Queries
{
    public class GetChannelSlug(string slug) : IQuery<ChannelSlug?>
    {
        public async Task<ChannelSlug?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.ChannelSlugs.WithPartitionKey(slug).FirstOrDefaultAsync(channelSlug => channelSlug.Slug == slug);
        }
    }
}
