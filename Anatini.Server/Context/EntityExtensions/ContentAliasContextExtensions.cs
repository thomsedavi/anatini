using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class ContentAliasContextExtensions
    {
        public static async Task<ContentAlias> AddContentAliasAsync(this AnatiniContext context, Guid contentId, Guid channelId, string slug, string contentName)
        {
            var contentAlias = new ContentAlias
            {
                ItemId = ItemId.Get(channelId, slug),
                Slug = slug,
                ContentChannelId = channelId,
                ContentId = contentId,
                ContentName = contentName
            };

            await context.Add(contentAlias);

            return contentAlias;
        }
    }
}
