using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ChannelAliasContextExtensions
    {
        public static async Task<ChannelAlias> AddChannelAliasAsync(this AnatiniContext context, string handle, string channelId, string channelName, bool? @protected)
        {
            var channelAlias = new ChannelAlias
            {
                ItemId = ItemId.Get(handle),
                Handle = handle,
                ChannelId = channelId,
                ChannelName = channelName,
                Protected = @protected
            };

            await context.AddAsync(channelAlias);

            return channelAlias;
        }
    }
}
