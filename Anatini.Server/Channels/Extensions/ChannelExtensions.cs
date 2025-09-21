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
                Name = post.Name
            };

            var posts = channel.Posts ?? [];
            posts.Add(channelOwnedPost);
            channel.Posts = posts;

            return channel;
        }

        public static ChannelDto ToChannelDto(this Channel channel)
        {
            var channelDto = new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Posts = channel.Posts?.Select(ToChannelPostDto)
            };

            return channelDto;
        }

        public static ChannelPostDto ToChannelPostDto(this ChannelOwnedPost channelOwnedPost)
        {
            var channelPostDto = new ChannelPostDto
            {
                Id = channelOwnedPost.Id,
                Name = channelOwnedPost.Name
            };

            return channelPostDto;
        }
    }
}
