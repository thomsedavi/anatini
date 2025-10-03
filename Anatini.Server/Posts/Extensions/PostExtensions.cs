using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Posts.Extensions
{
    public static class PostExtensions
    {
        public static PostDto ToPostDto(this Post post)
        {
            return new PostDto
            {
                Name = post.Name
            };
        }
    }
}
