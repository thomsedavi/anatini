using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ContentAliasContextExtensions
    {
        public static async Task<ContentAlias> AddContentAliasAsync(this AnatiniContext context, string contentId, string channelId, string handle, string contentName, bool? @protected)
        {
            var contentAlias = new ContentAlias
            {
                ItemId = ItemId.Get(channelId, handle),
                Handle = handle,
                ContentChannelId = channelId,
                ContentId = contentId,
                ContentName = contentName,
                Protected = @protected
            };

            await context.AddAsync(contentAlias);

            return contentAlias;
        }
    }
}
