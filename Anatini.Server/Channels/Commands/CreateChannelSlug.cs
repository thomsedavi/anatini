using Anatini.Server.Interfaces;

namespace Anatini.Server.Channels.Commands
{
    public class CreateChannelSlug(Guid id, string slug, Guid channelId, string channelName) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var channelSlug = new ChannelSlug
            {
                Id = id,
                Slug = slug,
                ChannelId = channelId,
                ChannelName = channelName
            };

            context.Add(channelSlug);

            return await context.SaveChangesAsync();
        }
    }
}
