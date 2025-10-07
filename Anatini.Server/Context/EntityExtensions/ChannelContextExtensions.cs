namespace Anatini.Server.Context.EntityExtensions
{
    public static class ChannelContextExtensions
    {
        public static async Task<Channel> AddChannelAsync(this AnatiniContext context, string id, string name, string slug, string userId, string userName)
        {
            var channelOwnedUser = new ChannelOwnedUser
            {
                Id = userId,
                Name = userName,
                ChannelId = id
            };

            var channelOwnedAlias = new ChannelOwnedAlias
            {
                Slug = slug,
                ChannelId = id
            };

            var channel = new Channel
            {
                ItemId = id,
                Id = id,
                Name = name,
                DefaultSlug = slug,
                Aliases = [channelOwnedAlias],
                Users = [channelOwnedUser]
            };

            await context.Add(channel);

            return channel;
        }
    }
}
