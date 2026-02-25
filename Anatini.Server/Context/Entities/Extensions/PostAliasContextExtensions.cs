using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostAliasContextExtensions
    {
        public static async Task<PostAlias> AddPostAliasAsync(this AnatiniContext context, string postId, string channelId, string handle, string postName, bool? @protected)
        {
            var postAlias = new PostAlias
            {
                ItemId = ItemId.Get(channelId, handle),
                Handle = handle,
                PostChannelId = channelId,
                PostId = postId,
                PostName = postName,
                Protected = @protected
            };

            await context.AddAsync(postAlias);

            return postAlias;
        }
    }
}
