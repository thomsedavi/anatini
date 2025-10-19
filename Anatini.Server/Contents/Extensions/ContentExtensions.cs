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
                Name = content.DraftVersion.Name,
                DefaultSlug = content.DefaultSlug
            };
        }
    }
}
