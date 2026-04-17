using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelExtensions
    {
        public static ChannelDto ToChannelDto(this Channel channel)
        {
            return new ChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                Handle = channel.Handle
            };
        }

        public static ChannelEditDto ToChannelEditDto(this Channel channel)
        {
            return new ChannelEditDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                Handle = channel.Handle,
                Visibility = channel.Visibility.ToString()
            };
        }
    }
}
