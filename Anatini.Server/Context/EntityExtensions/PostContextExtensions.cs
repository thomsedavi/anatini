using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class PostContextExtensions
    {
        public static async Task<Post> AddPostAsync(this AnatiniContext context, Guid id, string name, string slug, Guid channelId, EventData eventData)
        {
            var postOwnedSlug = new PostOwnedAlias
            {
                Slug = slug,
                PostChannelId = channelId,
                PostId = id
            };

            var post = new Post
            {
                ItemId = ItemId.Get(channelId, id),
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
