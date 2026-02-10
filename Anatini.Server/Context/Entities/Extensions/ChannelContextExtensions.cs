using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ChannelContextExtensions
    {
        public static async Task<Channel> AddChannelAsync(this AnatiniContext context, string id, string name, string handle, bool? @protected, string userId, string userName)
        {
            var channelOwnedUser = new ChannelOwnedUser
            {
                Id = userId,
                Name = userName,
                ChannelId = id
            };

            var channelOwnedAlias = new ChannelOwnedAlias
            {
                Handle = handle,
                ChannelId = id
            };

            var channel = new Channel
            {
                ItemId = ItemId.Get(id),
                Id = id,
                Name = name,
                DefaultHandle = handle,
                Aliases = [channelOwnedAlias],
                Users = [channelOwnedUser],
                Protected = @protected
            };

            await context.AddAsync(channel);

            return channel;
        }
    }
}
