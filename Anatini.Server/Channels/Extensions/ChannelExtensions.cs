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
                Slug = post.Slugs.First(slug => slug.Id == post.DefaultSlugId).Slug,
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
                Id = post.Id,
                ChannelId = channel.Id,
                Name = post.Name,
                Slug = post.Slugs.First(slug => slug.Id == post.DefaultSlugId).Slug,
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
                Slug = channel.Slugs.First(slug => slug.Id == channel.DefaultSlugId).Slug
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
                Id = channel.Id,
                Name = channel.Name,
                TopDraftPosts = channel.TopDraftPosts?.Select(ToChannelEditPostDto),
                Slugs = channel.Slugs.Select(ToChannelEditSlugDto),
                DefaultSlugId = channel.DefaultSlugId
            };
        }

        public static ChannelEditPostDto ToChannelEditPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            return new ChannelEditPostDto
            {
                Id = channelOwnedPost.Id,
                Name = channelOwnedPost.Name,
                Slug = channelOwnedPost.Slug,
                UpdatedDateUTC = channelOwnedPost.UpdatedDateUTC
            };
        }

        public static ChannelEditSlugDto ToChannelEditSlugDto(this ChannelOwnedSlug channelOwnedSlug)
        {
            return new ChannelEditSlugDto
            {
                Id = channelOwnedSlug.Id,
                Slug = channelOwnedSlug.Slug
            };
        }
    }
}
