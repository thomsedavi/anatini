using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Contents.Extensions
{
    public static class ContentExtensions
    {
        public static ContentDto ToContentDto(this Content content)
        {
            return new ContentDto
            {
                Name = content.PublishedVersion?.Name ?? "(unknown)"
            };
        }

        public static ContentEditDto ToContentEditDto(this Content content)
        {
            return new ContentEditDto
            {
                Id = content.Id,
                ChannelId = content.ChannelId,
                DefaultSlug = content.DefaultSlug,
                DraftVersion = content.DraftVersion.ToContentVersionDto()
            };
        }

        public static ContentVersionDto ToContentVersionDto(this ContentOwnedVersion contentOwnedVersion)
        {
            return new ContentVersionDto
            {
                Name = contentOwnedVersion.Name,
                Elements = contentOwnedVersion.Elements?.Select(ToContentElementDto),
            };
        }

        public static ContentElementDto ToContentElementDto(this ContentOwnedElement contentOwnedElement)
        {
            return new ContentElementDto
            {
                Tag = contentOwnedElement.Tag,
                Index = contentOwnedElement.Index,
                Content = contentOwnedElement.Content
            };
        }
    }
}
