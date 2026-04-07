using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ChannelContextExtensions
    {
        public static Channel AddChannel(this ApplicationDbContext context, Guid userId, string handle, string normalizedHandle, string name, Visibility visibility)
        {
            var id = Guid.CreateVersion7();

            var channelHandle = new ChannelHandle
            {
                ChannelId = id,
                Handle = handle,
                NormalizedHandle = normalizedHandle
            };

            var userChannel = new ApplicationUserChannel
            {
                UserId = userId,
                ChannelId = id
            };

            var channel = new Channel
            {
                Id = id,
                Name = name,
                Handle = handle,
                NormalizedHandle = normalizedHandle,
                Visibility = visibility,
                Handles = [channelHandle],
                UserChannels = [userChannel]
            };

            context.Add(channel);

            return channel;
        }
    }
}
