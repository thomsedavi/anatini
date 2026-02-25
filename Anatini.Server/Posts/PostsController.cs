using Anatini.Server.Posts.Extensions;
using Anatini.Server.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController : AnatiniControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] Query query) => await UsingContextAsync(async (context) =>
        {
            if (query.Week != null)
            {
                var value = $"{AttributePostType.Week}:{query.Week}";

                var attributePostsPage = await context.Context.AttributePosts.WithPartitionKey(value).OrderBy(a => a.ItemId).ToPageAsync(10, null);

                return Ok(new { AttributePosts = attributePostsPage.Values.Select(attributePost => attributePost.ToAttributePostDto()), attributePostsPage.ContinuationToken });
            }

            return BadRequest();
        });
    }

    public class Query
    {
        public string? Week { get; set; }
    }
}
