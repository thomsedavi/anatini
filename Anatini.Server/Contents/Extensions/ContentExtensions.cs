using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Contents.Extensions
{
    public static class ContentExtensions
    {
        public static ContentDto? ToContentDto(this Content content, bool usePreview = false)
        {
            if (usePreview)
            {
                return new ContentDto
                {
                    Version = content.DraftVersion.ToContentVersionDto()
                };
            }

            if (content.PublishedVersion == null)
            {
                return null;
            }

            return new ContentDto
            {
                Version = content.PublishedVersion.ToContentVersionDto()
            };
        }

        public static ContentEditDto ToContentEditDto(this Content content)
        {
            return new ContentEditDto
            {
                Id = content.Id,
                ChannelId = content.ChannelId,
                DefaultSlug = content.DefaultSlug,
                DateNZ = content.DateNZ,
                Version = content.DraftVersion.ToContentVersionDto()
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
