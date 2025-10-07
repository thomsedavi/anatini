using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class PostAliasContextExtensions
    {
        public static async Task<PostAlias> AddPostAliasAsync(this AnatiniContext context, string postId, string channelId, string slug, string postName)
        {
            var postAlias = new PostAlias
            {
                ItemId = IdGenerator.Get(channelId, slug),
                Slug = slug,
                ChannelId = channelId,
                PostId = postId,
                PostName = postName
            };

            await context.Add(postAlias);

            return postAlias;
        }
    }
}
