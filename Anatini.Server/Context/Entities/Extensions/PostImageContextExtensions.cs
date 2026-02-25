using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostImageContextExtensions
    {
        public static async Task<int> AddPostImageAsync(this AnatiniContext context, string id, string postId, string channelId)
        {
            var postImage = new PostImage
            {
                Id = id,
                ItemId = ItemId.Get(channelId, postId, id),
                PostChannelId = channelId,
                PostId = channelId
            };

            return await context.AddAsync(postImage);
        }
    }
}
