using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ContentImageContextExtensions
    {
        public static async Task<int> AddContentImageAsync(this AnatiniContext context, Guid id, Guid contentId, Guid channelId)
        {
            var contentImage = new ContentImage
            {
                Id = id,
                ItemId = ItemId.Get(channelId, contentId, id),
                ContentChannelId = channelId,
                ContentId = channelId
            };

            return await context.AddAsync(contentImage);
        }
    }
}
