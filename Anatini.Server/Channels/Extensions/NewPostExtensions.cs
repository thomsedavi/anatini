using Anatini.Server.Context;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewPostExtensions
    {
        public static Post Create(this NewPost newPost, Guid channelGuid, EventData eventData)
        {
            var postOwnedSlug = new PostOwnedAlias
            {
                Guid = newPost.AliasGuid,
                Slug = newPost.Slug,
                PostGuid = newPost.Guid
            };

            return new Post
            {
                Guid = newPost.Guid,
                ChannelGuid = channelGuid,
                Name = newPost.Name,
                DateNZ = eventData.DateOnlyNZNow,
                Aliases = [postOwnedSlug],
                DefaultAliasGuid = newPost.AliasGuid,
                UpdatedDateUTC = eventData.DateTimeUtc
            };
        }

        public static PostAlias CreateAlias(this NewPost newPost, Guid channelGuid)
        {
            return new PostAlias
            {
                Guid = newPost.AliasGuid,
                Slug = newPost.Slug,
                ChannelGuid = channelGuid,
                PostGuid = newPost.Guid,
                PostName = newPost.Name
            };
        }
    }
}
