using Anatini.Server.Context.Entities;
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
                DefaultHandle = content.DefaultHandle,
                Version = content.DraftVersion.ToContentVersionDto(),
                Protected = content.Protected,
                Status = content.Status
            };
        }

        public static ContentVersionDto ToContentVersionDto(this ContentOwnedVersion contentOwnedVersion)
        {
            return new ContentVersionDto
            {
                Name = contentOwnedVersion.Name,
                Article = contentOwnedVersion.Article,
                DateNZ = contentOwnedVersion.DateNZ
            };
        }
    }
}
