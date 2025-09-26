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
                Guid = post.Guid,
                ChannelGuid = channel.Guid,
                Name = post.Name,
                Slug = post.Aliases.First(alias => alias.Guid == post.DefaultAliasGuid).Slug,
                UpdatedDateUTC = eventData.DateTimeUtc
            };

            var topDraftPosts = channel.TopDraftPosts ?? [];
            topDraftPosts.Add(channelOwnedPost);
            channel.TopDraftPosts = [.. topDraftPosts.OrderByDescending(post => post.UpdatedDateUTC).Take(8)];

            return channel;
        }

        public static Channel AddPublishedPost(this Channel channel, Post post, EventData eventData)
        {
            var channelOwnedPost = new ChannelOwnedPost
            {
                Guid = post.Guid,
                ChannelGuid = channel.Guid,
                Name = post.Name,
                Slug = post.Aliases.First(alias => alias.Guid == post.DefaultAliasGuid).Slug,
                UpdatedDateUTC = eventData.DateTimeUtc
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
                Slug = channel.Aliases.First(alias => alias.Guid == channel.DefaultAliasGuid).Slug
            };
        }

        public static ChannelPostDto ToChannelPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelPostDto
            {
                Name = channelOwnedPost.Name,
                Slug = channelOwnedPost.Slug
            };
        }

        public static ChannelEditDto ToChannelEditDto(this Channel channel)
        {
            return new ChannelEditDto
            {
                Guid = channel.Guid,
                Name = channel.Name,
                TopDraftPosts = channel.TopDraftPosts?.Select(ToChannelEditPostDto),
                Aliases = channel.Aliases.Select(ToChannelEditAliasDto),
                DefaultAliasGuid = channel.DefaultAliasGuid
            };
        }

        public static ChannelEditPostDto ToChannelEditPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelEditPostDto
            {
                Guid = channelOwnedPost.Guid,
                Name = channelOwnedPost.Name,
                Slug = channelOwnedPost.Slug,
                UpdatedDateUTC = channelOwnedPost.UpdatedDateUTC
            };
        }

        public static ChannelEditAliasDto ToChannelEditAliasDto(this ChannelOwnedAlias channelOwnedAlias)
        {
            return new ChannelEditAliasDto
            {
                Guid = channelOwnedAlias.Guid,
                Slug = channelOwnedAlias.Slug
            };
        }
    }
}
