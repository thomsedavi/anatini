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

            var userChannelEdge = new ApplicationUserChannelEdge
            {
                UserId = userId,
                ChannelId = channelId,
                Label = UserChannelEdgeLabel.Owner,
                CreatedAtUtc = utcNow
            };

            var channel = new Channel
            {
                Id = channelId,
                Name = name,
                Handle = handle,
                Visibility = visibility,
                Handles = [channelHandle],
                UserEdges = [userChannelEdge],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(channel);

            return channel;
        }
    }
}
