using Anatini.Server.Context;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewChannelExtensions
    {
        public static Channel Create(this NewChannel newChannel, User user)
        {
            var channelOwnedUser = new ChannelOwnedUser
            {
                Id = user.Id,
                Name = user.Name,
                ChannelId = newChannel.Id,
            };

            var channelOwnedAlias = new ChannelOwnedAlias
            {
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id
            };

            return new Channel
            {
                Id = newChannel.Id,
                Name = newChannel.Name,
                Users = [channelOwnedUser],
                Aliases = [channelOwnedAlias],
                DefaultSlug = newChannel.Slug
            };
        }

        public static ChannelAlias CreateAlias(this NewChannel newChannel)
        {
            return new ChannelAlias
            {
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id,
                ChannelName = newChannel.Name
            };
        }
    }
}
