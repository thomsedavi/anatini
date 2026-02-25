using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Posts.Extensions
{
    public static class PostExtensions
    {
        public static PostDto? ToPostDto(this Post post, bool usePreview = false)
        {
            if (usePreview)
            {
                return new PostDto
                {
                    Version = post.DraftVersion.ToPostVersionDto()
                };
            }

            if (post.PublishedVersion == null)
            {
                return null;
            }

            return new PostDto
            {
                Version = post.PublishedVersion.ToPostVersionDto()
            };
        }

        public static PostEditDto ToPostEditDto(this Post post)
        {
            return new PostEditDto
            {
                Id = post.Id,
                ChannelId = post.ChannelId,
                DefaultHandle = post.DefaultHandle,
                Version = post.DraftVersion.ToPostVersionDto(),
                Protected = post.Protected,
                Status = post.Status
            };
        }

        public static PostVersionDto ToPostVersionDto(this PostOwnedVersion postOwnedVersion)
        {
            return new PostVersionDto
            {
                Name = postOwnedVersion.Name,
                Article = postOwnedVersion.Article,
                DateNZ = postOwnedVersion.DateNZ
            };
        }
    }
}
