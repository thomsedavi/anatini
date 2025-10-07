using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class PostContextExtensions
    {
        public static async Task<Post> AddPostAsync(this AnatiniContext context, string id, string name, string slug, string channelId, EventData eventData)
        {
            var postOwnedSlug = new PostOwnedAlias
            {
                Slug = slug,
                ChannelId = channelId,
                PostId = id
            };

            var post = new Post
            {
                ItemId = IdGenerator.Get(channelId, id),
                Id = id,
                ChannelId = channelId,
                Name = name,
                DateOnlyNZ = eventData.DateOnlyNZNow,
                Aliases = [postOwnedSlug],
                DefaultSlug = slug,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            await context.Add(post);

            return post;
        }
    }
}
