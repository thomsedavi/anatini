using Anatini.Server.Interfaces;

namespace Anatini.Server.Channels.Commands
{
    internal class CreateChannel(Guid id, string name, Guid userId, string userName, Guid slugId, string slug) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var channelUser = new ChannelOwnedUser
            {
                UserId = userId,
                UserName = userName,
                ChannelId = id,
            };

            var channelSlug = new ChannelOwnedSlug
            {
                SlugId = slugId,
                Slug = slug,
                ChannelId = id
            };

            var channel = new Channel
            {
                Id = id,
                Name = name,
                Users = [channelUser],
                Slugs = [channelSlug],
            };

            context.Add(channel);

            return await context.SaveChangesAsync();
        }
    }
}
