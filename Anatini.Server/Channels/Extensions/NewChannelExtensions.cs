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

            var channelOwnedSlug = new ChannelOwnedSlug
            {
                Id = newChannel.SlugId,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id
            };

            return new Channel
            {
                Id = newChannel.Id,
                Name = newChannel.Name,
                Users = [channelOwnedUser],
                Slugs = [channelOwnedSlug],
                DefaultSlugId = newChannel.SlugId
            };
        }

        public static ChannelSlug CreateSlug(this NewChannel newChannel)
        {
            return new ChannelSlug
            {
                Id = newChannel.SlugId,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id,
                ChannelName = newChannel.Name
            };
        }
    }
}
