using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Channels.Extensions
{
    public static class ChannelSlugExtensions
    {
        public static ChannelDto ToChannelDto(this ChannelSlug channelSlug)
        {
            var channelDto = new ChannelDto
            {
                Id = channelSlug.ChannelId,
                Name = channelSlug.ChannelName
            };

            return channelDto;
        }
    }
}
