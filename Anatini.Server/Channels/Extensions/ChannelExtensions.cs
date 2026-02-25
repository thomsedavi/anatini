using Anatini.Server.Context.Entities;
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
                Name = post.DraftVersion.Name,
                DefaultHandle = post.DefaultHandle,
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
                Name = post.PublishedVersion?.Name ?? "(unknown)",
                DefaultHandle = post.DefaultHandle,
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
                Id = channel.Id,
                Name = channel.Name,
                TopPosts = channel.TopPublishedPosts?.Select(ToChannelPostDto),
                DefaultHandle = channel.DefaultHandle
            };
        }

        public static ChannelPostDto ToChannelPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelPostDto
            {
                Name = channelOwnedPost.Name,
                DefaultHandle = channelOwnedPost.DefaultHandle
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
                DefaultCardImageId = channel.DefaultCardImageId,
                DefaultHandle = channel.DefaultHandle,
                Protected = channel.Protected
            };
        }

        public static ChannelEditPostDto ToChannelEditPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelEditPostDto
            {
                Id = channelOwnedPost.Id,
                Name = channelOwnedPost.Name,
                DefaultHandle = channelOwnedPost.DefaultHandle,
                UpdatedDateTimeUTC = channelOwnedPost.UpdatedDateTimeUTC
            };
        }

        public static ChannelEditAliasDto ToChannelEditAliasDto(this ChannelOwnedAlias channelOwnedAlias)
        {
            return new ChannelEditAliasDto
            {
                Handle = channelOwnedAlias.Handle
            };
        }
    }
}
