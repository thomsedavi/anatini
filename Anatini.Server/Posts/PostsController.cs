using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
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
            var eventData = new EventData(HttpContext);

            async Task<IActionResult> channelFunction(Channel channel)
            {
                var postAlias = newPost.CreateAlias(channel.Id);

                // Returns Conflict if channel slug already exists
                await new Add(postAlias).ExecuteAsync();

                var post = newPost.Create(channel.Id, eventData);

                channel.AddDraftPost(post, eventData);

                await new Add(post).ExecuteAsync();
                await new Update(channel).ExecuteAsync();

                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannel(channelSlug, true, channelFunction);
        }

        [HttpGet("{postSlug}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string channelSlug, string postSlug)
        {
            return await Task.FromResult(Ok(new { channelSlug, postSlug }));
        }
    }
}
