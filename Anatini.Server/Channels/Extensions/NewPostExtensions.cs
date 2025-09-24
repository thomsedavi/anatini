using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewPostExtensions
    {
        public static Post Create(this NewPost newPost, EventData eventData)
        {
            var postOwnedSlug = new PostOwnedSlug
            {
                Id = newPost.SlugId,
                Slug = newPost.Slug,
                PostId = newPost.Id
            };

            return new Post
            {
                Id = newPost.Id,
                Name = newPost.Name,
                DateNZ = eventData.DateOnlyNZNow,
                Slugs = [postOwnedSlug],
                DefaultSlugId = newPost.SlugId
            };
        }

        public static PostSlug CreateSlug(this NewPost newPost, Guid channelId)
        {
            return new PostSlug
            {
                Id = newPost.SlugId,
                Slug = newPost.Slug,
                ChannelId = channelId,
                PostId = newPost.Id,
                PostName = newPost.Name
            };
        }
    }
}
