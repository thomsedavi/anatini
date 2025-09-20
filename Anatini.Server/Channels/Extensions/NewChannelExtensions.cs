using Anatini.Server.Context;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewChannelExtensions
    {
        public static Channel Create(this NewChannel newChannel, User user)
        {
            var channelOwnedUser = new ChannelOwnedUser
            {
                UserId = user.Id,
                UserName = user.Name,
                ChannelId = newChannel.Id,
            };

            var channelOwnedSlug = new ChannelOwnedSlug
            {
                SlugId = newChannel.SlugId,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id
            };

            var channel = new Channel
            {
                Id = newChannel.Id,
                Name = newChannel.Name,
                Users = [channelOwnedUser],
                Slugs = [channelOwnedSlug],
            };

            return channel;
        }

        public static ChannelSlug CreateSlug(this NewChannel newChannel)
        {
            var channelSlug = new ChannelSlug
            {
                Id = newChannel.SlugId,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id,
                ChannelName = newChannel.Name
            };

            return channelSlug;
        }
    }
}
