using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
using Anatini.Server.Posts.Extensions;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPost([FromForm] NewPost newPost)
        {
            var eventData = new EventData(HttpContext);

            async Task<IActionResult> channelFunction(Channel channel)
            {
                var postSlug = newPost.CreateSlug();

                // Returns Conflict if channel slug already exists
                await new Add(postSlug).ExecuteAsync();

                var post = newPost.Create(eventData);

                channel.AddPost(post);

                await new Add(post).ExecuteAsync();
                await new Update(channel).ExecuteAsync();

                return Ok(channel.ToChannelDto());
            }

            return await UsingChannel(newPost.ChannelId, channelFunction);
        }
    }
}
