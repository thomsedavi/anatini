using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ChannelContextExtensions
    {
        public static Channel AddChannel(this ApplicationDbContext context, Guid userId, string handle, string name, Visibility visibility)
        {
            var channelId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var channelHandle = new ChannelHandle
            {
                Id = Guid.CreateVersion7(),
                ChannelId = channelId,
                Handle = handle,
                CreatedAtUtc = utcNow
            };

            var userChannel = new ApplicationUserChannel
            {
                UserId = userId,
                ChannelId = channelId,
                CreatedAtUtc = utcNow
            };

            var channel = new Channel
            {
                Id = channelId,
                Name = name,
                Handle = handle,
                Visibility = visibility,
                Handles = [channelHandle],
                UserChannels = [userChannel],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(channel);

            return channel;
        }
    }
}
