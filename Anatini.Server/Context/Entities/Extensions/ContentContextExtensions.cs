using System.Xml.Linq;
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
                ContentHandle = content.DefaultHandle,
                ContentChannelId = channel.Id,
                ContentChannelHandle = channel.DefaultHandle,
                ContentName = content.DraftVersion.Name,
                ChannelName = channel.Name,
                ChannelDefaultCardImageId = channel.DefaultCardImageId,
                CardImageId = content.DraftVersion.CardImageId
            };

            await context.AddAsync(attributeContent);

            return attributeContent;
        }

        public static async Task<Content> AddContentAsync(this AnatiniContext context, string id, string name, string handle, bool? @protected, string channelId, EventData eventData)
        {
            var article = new XElement("article", new XElement("header", new XElement("h1", new XAttribute("tabindex", -1), name)));

            var channelOwnedAlias = new ContentOwnedAlias
            {
                Handle = handle,
                ContentChannelId = channelId,
                ContentId = id
            };

            var channelOwnedDraftVersion = new ContentOwnedVersion
            {
                ContentChannelId = channelId,
                Name = name,
                ContentId = id,
                DateNZ = eventData.DateOnlyNZNow,
                Article = article.ToString()
            };

            var content = new Content
            {
                ItemId = ItemId.Get(channelId, id),
                Id = id,
                Status = "Draft",
                ContentType = "Post",
                ChannelId = channelId,
                Aliases = [channelOwnedAlias],
                DefaultHandle = handle,
                DraftVersion = channelOwnedDraftVersion,
                UpdatedDateTimeUTC = eventData.DateTimeUtc,
                Protected = @protected
            };

            await context.AddAsync(content);

            return content;
        }
    }
}
