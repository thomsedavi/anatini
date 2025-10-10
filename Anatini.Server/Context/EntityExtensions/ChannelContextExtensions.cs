﻿using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class ChannelContextExtensions
    {
        public static async Task<Channel> AddChannelAsync(this AnatiniContext context, Guid id, string name, string slug, Guid userId, string userName)
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
                ItemId = ItemId.Get(id),
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
