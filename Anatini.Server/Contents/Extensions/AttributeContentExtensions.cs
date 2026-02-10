using Anatini.Server.Context.Entities;
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
                ContentHandle = attributeContent.ContentHandle,
                ContentChannelHandle = attributeContent.ContentChannelHandle
            };
        }
    }
}
