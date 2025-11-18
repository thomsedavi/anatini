using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Contents.Extensions
{
    public static class AttributeContentExtensions
    {
        public static AttributeContentDto ToAttributeContentDto(this AttributeContent attributeContent)
        {
            return new AttributeContentDto
            {
                Name = attributeContent.ContentName,
                ContentSlug = attributeContent.ContentSlug,
                ContentChannelSlug = attributeContent.ContentChannelSlug
            };
        }
    }
}
