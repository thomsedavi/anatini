using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts.Extensions
{
    public static class NewPostExtensions
    {
        public static Post Create(this NewPost newPost, string channelId, EventData eventData)
        {
            var postOwnedSlug = new PostOwnedAlias
            {
                Slug = newPost.Slug,
                ChannelId = channelId,
                PostId = newPost.Id
            };

            return new Post
            {
                Id = newPost.Id,
                ChannelId = channelId,
                Name = newPost.Name,
                DateOnlyNZ = eventData.DateOnlyNZNow,
                Aliases = [postOwnedSlug],
                DefaultSlug = newPost.Slug,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };
        }

        public static PostAlias CreateAlias(this NewPost newPost, string channelId)
        {
            return new PostAlias
            {
                Id = IdGenerator.Get(channelId, newPost.Slug),
                Slug = newPost.Slug,
                ChannelId = channelId,
                PostId = newPost.Id,
                PostName = newPost.Name
            };
        }
    }
}
