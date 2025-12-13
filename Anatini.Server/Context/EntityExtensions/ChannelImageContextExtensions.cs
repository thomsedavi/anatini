using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class ChannelImageContextExtensions
    {
        public static async Task<int> AddChannelImageAsync(this AnatiniContext context, Guid id, Guid channelId)
        {
            var channelImage = new ChannelImage
            {
                Id = id,
                ItemId = ItemId.Get(channelId, id),
                ChannelId = channelId
            };

            return await context.Add(channelImage);
        }
    }
}
