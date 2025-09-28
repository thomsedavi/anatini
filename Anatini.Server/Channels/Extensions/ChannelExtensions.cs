using Anatini.Server.Context;
using Anatini.Server.Dtos;
using Anatini.Server.Utils;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelExtensions
    {
        public static Channel AddDraftPost(this Channel channel, Post post, EventData eventData)
        {
            var channelOwnedPost = new ChannelOwnedPost
            {
                Id = post.Id,
                ChannelId = channel.Id,
                Name = post.Name,
                DefaultSlug = post.DefaultSlug,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            var topDraftPosts = channel.TopDraftPosts ?? [];
            topDraftPosts.Add(channelOwnedPost);
            channel.TopDraftPosts = [.. topDraftPosts.OrderByDescending(post => post.UpdatedDateTimeUTC).Take(8)];

            return channel;
        }

        public static Channel AddPublishedPost(this Channel channel, Post post, EventData eventData)
        {
            var channelOwnedPost = new ChannelOwnedPost
            {
                Id = post.Id,
                ChannelId = channel.Id,
                Name = post.Name,
                DefaultSlug = post.DefaultSlug,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            // TODO top 8
            var topPublishedPosts = channel.TopPublishedPosts ?? [];
            topPublishedPosts.Add(channelOwnedPost);
            channel.TopPublishedPosts = topPublishedPosts;

            return channel;
        }

        public static ChannelDto ToChannelDto(this Channel channel)
        {
            return new ChannelDto
            {
                Name = channel.Name,
                TopPosts = channel.TopPublishedPosts?.Select(ToChannelPostDto),
                DefaultSlug = channel.DefaultSlug
            };
        }

        public static ChannelPostDto ToChannelPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelPostDto
            {
                Name = channelOwnedPost.Name,
                DefaultSlug = channelOwnedPost.DefaultSlug
            };
        }

        public static ChannelEditDto ToChannelEditDto(this Channel channel)
        {
            return new ChannelEditDto
            {
                Id = channel.Id,
                Name = channel.Name,
                TopDraftPosts = channel.TopDraftPosts?.Select(ToChannelEditPostDto),
                Aliases = channel.Aliases.Select(ToChannelEditAliasDto),
                DefaultSlug = channel.DefaultSlug
            };
        }

        public static ChannelEditPostDto ToChannelEditPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelEditPostDto
            {
                Id = channelOwnedPost.Id,
                Name = channelOwnedPost.Name,
                DefaultSlug = channelOwnedPost.DefaultSlug,
                UpdatedDateTimeUTC = channelOwnedPost.UpdatedDateTimeUTC
            };
        }

        public static ChannelEditAliasDto ToChannelEditAliasDto(this ChannelOwnedAlias channelOwnedAlias)
        {
            return new ChannelEditAliasDto
            {
                Slug = channelOwnedAlias.Slug
            };
        }
    }
}
