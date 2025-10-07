using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
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
            async Task<IActionResult> channelContextFunctionAsync(Channel channel, AnatiniContext context)
            {
                var eventData = new EventData(HttpContext);

                var postAlias = await context.AddPostAliasAsync(newPost.Id, channel.Id, newPost.Slug, newPost.Name);
                var post = await context.AddPostAsync(newPost.Id, newPost.Name, newPost.Slug, channel.Id, eventData);

                channel.AddDraftPost(post, eventData);
                await context.Update(channel);

                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannelContextAsync(channelSlug, channelContextFunctionAsync, requiresAuthorisation: true);
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
