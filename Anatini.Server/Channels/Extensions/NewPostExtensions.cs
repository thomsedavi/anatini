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

            var post = new Post
            {
                Id = newPost.Id,
                Name = newPost.Name,
                DateNZ = eventData.DateOnlyNZNow,
                Slugs = [postOwnedSlug],
            };

            return post;
        }

        public static PostSlug CreateSlug(this NewPost newPost, Guid channelId)
        {
            var channelSlug = new PostSlug
            {
                Id = newPost.SlugId,
                Slug = newPost.Slug,
                ChannelId = channelId,
                PostId = newPost.Id,
                PostName = newPost.Name
            };

            return channelSlug;
        }
    }
}
