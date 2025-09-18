using Anatini.Server.Context;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewChannelExtensions
    {
        public static Channel Create(this NewChannel newChannel, NewChannelSlug newChannelSlug, User user)
        {
            var channelUser = new ChannelOwnedUser
            {
                UserId = user.Id,
                UserName = user.Name,
                ChannelId = newChannel.Id,
            };

            var channelSlug = new ChannelOwnedSlug
            {
                SlugId = newChannelSlug.Id,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id
            };

            var channel = new Channel
            {
                Id = newChannel.Id,
                Name = newChannel.Name,
                Users = [channelUser],
                Slugs = [channelSlug],
            };

            return channel;
        }
    }
}
