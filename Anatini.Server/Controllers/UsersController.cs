using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController() : ControllerBase
    {
        [HttpGet("{userHandle}/posts/{postHandle}")]
        [ProducesResponseType<PostResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserPost(string userHandle, string postHandle)
        {
            var response = new PostResponse
            {
                Post = $"users/{userHandle}/posts/{postHandle}",
            };

            return Ok(response);
        }
    }

    internal class PostResponse
    {
        public required string Post { get; set; }
    }
}
