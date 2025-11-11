using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class ContentContextExtensions
    {
        public static async Task<Content> AddContentAsync(this AnatiniContext context, Guid id, string name, string slug, Guid channelId, EventData eventData)
        {
            var channelOwnedAlias = new ContentOwnedAlias
            {
                Slug = slug,
                ContentChannelId = channelId,
                ContentId = id
            };

            var channelOwnedDraftVersion = new ContentOwnedVersion
            {
                ContentChannelId = channelId,
                Name = name,
                ContentId = id
            };

            var content = new Content
            {
                ItemId = ItemId.Get(channelId, id),
                Id = id,
                Status = "Draft",
                ContentType = "Post",
                ChannelId = channelId,
                DateNZ = eventData.DateOnlyNZNow,
                Aliases = [channelOwnedAlias],
                DefaultSlug = slug,
                DraftVersion = channelOwnedDraftVersion,
                UpdatedDateTimeUTC = eventData.DateTimeUtc
            };

            await context.Add(content);

            return content;
        }
    }
}
