using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewPostExtensions
    {
        public static Post Create(this NewPost newPost, Guid channelId, EventData eventData)
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

        public static PostAlias CreateAlias(this NewPost newPost, Guid channelId)
        {
            return new PostAlias
            {
                Slug = newPost.Slug,
                ChannelId = channelId,
                PostId = newPost.Id,
                PostName = newPost.Name
            };
        }
    }
}
