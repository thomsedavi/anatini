using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class ChannelAliasContextExtensions
    {
        public static async Task<ChannelAlias> AddChannelAliasAsync(this AnatiniContext context, string slug, Guid channelId, string channelName)
        {
            var channelAlias = new ChannelAlias
            {
                ItemId = ItemId.Get(slug),
                Slug = slug,
                ChannelId = channelId,
                ChannelName = channelName
            };

            await context.Add(channelAlias);

            return channelAlias;
        }
    }
}
