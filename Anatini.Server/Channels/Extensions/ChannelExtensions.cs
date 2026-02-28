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
                DefaultHandle = channel.DefaultHandle
            };
        }

        public static ChannelEditDto ToChannelEditDto(this Channel channel)
        {
            return new ChannelEditDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Aliases = channel.Aliases.Select(ToChannelEditAliasDto),
                DefaultCardImageId = channel.DefaultCardImageId,
                DefaultHandle = channel.DefaultHandle,
                Protected = channel.Protected
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
