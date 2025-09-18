using Anatini.Server.Context;

namespace Anatini.Server.Channels.Extensions
{
    public static class NewChannelSlugExtensions
    {
        public static ChannelSlug Create(this NewChannelSlug newChannelSlug, NewChannel newChannel)
        {
            var channelSlug = new ChannelSlug
            {
                Id = newChannelSlug.Id,
                Slug = newChannel.Slug,
                ChannelId = newChannel.Id,
                ChannelName = newChannel.Name
            };

            return channelSlug;
        }
    }
}
