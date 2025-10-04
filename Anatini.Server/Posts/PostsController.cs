using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Posts.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/channels/{channelSlug}/[controller]")]
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
        public async Task<IActionResult> PostPost(string channelSlug, [FromForm] NewPost newPost)
        {
            IActionResult channelContextFunction(Channel channel, AnatiniContext context)
            {
                var eventData = new EventData(HttpContext);

                var postAlias = newPost.CreateAlias(channel.Id);
                var post = newPost.Create(channel.Id, eventData);
                context.AddRange(postAlias, post);

                channel.AddDraftPost(post, eventData);
                context.Update(channel);

                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannelContext(channelSlug, channelContextFunction, requiresAuthorisation: true);
        }

        [HttpGet("{postSlug}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string channelSlug, string postSlug)
        {
            IActionResult postFunction(Post post)
            {
                return Ok(post.ToPostDto());
            }

            return await UsingPost(channelSlug, postSlug, postFunction);
        }
    }
}
