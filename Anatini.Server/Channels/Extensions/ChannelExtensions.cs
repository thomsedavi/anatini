using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelExtensions
    {
        public static Channel AddPost(this Channel channel, Post post)
        {
            var channelOwnedPost = new ChannelOwnedPost
            {
                Id = post.Id,
                ChannelId = channel.Id,
                Name = post.Name,
                Slug = post.Slugs.First(slug => slug.Id == post.DefaultSlugId).Slug
            };

            var posts = channel.Posts ?? [];
            posts.Add(channelOwnedPost);
            channel.Posts = posts;

            return channel;
        }

        public static ChannelDto ToChannelDto(this Channel channel)
        {
            return new ChannelDto
            {
                Name = channel.Name,
                Posts = channel.Posts?.Select(ToChannelPostDto),
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
                Posts = channel.Posts?.Select(ToChannelEditPostDto),
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
                Slug = channelOwnedPost.Slug
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
