using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class ContentContextExtensions
    {
        public static async Task<AttributeContent> AddAttributeContent(this AnatiniContext context, AttributeContentType attributeType, string attributeValue, Channel channel, Content content)
        {
            var value = $"{Enum.GetName(attributeType)!}:{attributeValue}";

            var attributeContent = new AttributeContent
            {
                ItemId = ItemId.Get(value, channel.Id, content.Id),
                Value = value,
                ContentId = content.Id,
                ContentSlug = content.DefaultSlug,
                ContentChannelId = channel.Id,
                ContentChannelSlug = channel.DefaultSlug,
                ContentName = content.DraftVersion.Name,
                ChannelName = channel.Name
            };

            await context.Add(attributeContent);

            return attributeContent;
        }

        public static async Task<Content> AddContentAsync(this AnatiniContext context, Guid id, string name, string slug, bool? @protected, Guid channelId, EventData eventData)
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
                ContentId = id,
                DateNZ = eventData.DateOnlyNZNow,
            };

            var content = new Content
            {
                ItemId = ItemId.Get(channelId, id),
                Id = id,
                Status = "Draft",
                ContentType = "Post",
                ChannelId = channelId,
                Aliases = [channelOwnedAlias],
                DefaultSlug = slug,
                DraftVersion = channelOwnedDraftVersion,
                UpdatedDateTimeUTC = eventData.DateTimeUtc,
                Protected = @protected
            };

            await context.Add(content);

            return content;
        }
    }
}
