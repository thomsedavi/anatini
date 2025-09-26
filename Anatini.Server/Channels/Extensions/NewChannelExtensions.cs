using Anatini.Server.Context;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewChannelExtensions
    {
        public static Channel Create(this NewChannel newChannel, User user)
        {
            var channelOwnedUser = new ChannelOwnedUser
            {
                Guid = user.Guid,
                Name = user.Name,
                ChannelGuid = newChannel.Guid,
            };

            var channelOwnedAlias = new ChannelOwnedAlias
            {
                Guid = newChannel.AliasGuid,
                Slug = newChannel.Slug,
                ChannelGuid = newChannel.Guid
            };

            return new Channel
            {
                Guid = newChannel.Guid,
                Name = newChannel.Name,
                Users = [channelOwnedUser],
                Aliases = [channelOwnedAlias],
                DefaultAliasGuid = newChannel.AliasGuid
            };
        }

        public static ChannelAlias CreateSlug(this NewChannel newChannel)
        {
            return new ChannelAlias
            {
                Guid = newChannel.AliasGuid,
                Slug = newChannel.Slug,
                ChannelGuid = newChannel.Guid,
                ChannelName = newChannel.Name
            };
        }
    }
}
