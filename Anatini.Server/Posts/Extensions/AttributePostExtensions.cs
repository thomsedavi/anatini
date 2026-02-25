using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Posts.Extensions
{
    public static class AttributePostExtensions
    {
        public static AttributePostDto ToAttributePostDto(this AttributePost attributePost)
        {
            return new AttributePostDto
            {
                Name = attributePost.PostName,
                PostHandle = attributePost.PostHandle,
                PostChannelHandle = attributePost.PostChannelHandle,
                DateNZ = attributePost.DateNZ
            };
        }
    }
}
