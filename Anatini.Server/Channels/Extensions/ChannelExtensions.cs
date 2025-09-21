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
                ChannelId = post.Id,
                Name = post.Name
            };

            var channels = channel.Posts ?? [];
            channels.Add(channelOwnedPost);
            channel.Posts = channels;

            return channel;
        }

        public static ChannelDto ToChannelDto(this Channel channel)
        {
            var channelDto = new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name
            };

            return channelDto;
        }
    }
}
