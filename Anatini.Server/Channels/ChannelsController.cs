using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost("{channelSlug}/posts")]
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
                var postSlug = newPost.CreateSlug(channel.Id);

                // Returns Conflict if channel slug already exists
                await new Add(postSlug).ExecuteAsync();

                var post = newPost.Create(channel.Id, eventData);

                // TODO Don't actually do this yet, channel only retains the eight most recent published posts
                channel.AddPost(post);

                await new Add(post).ExecuteAsync();
                await new Update(channel).ExecuteAsync();

                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannel(channelSlug, true, channelFunction);
        }

        [HttpGet("{channelSlug}/posts/{postSlug}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string channelSlug, string postSlug)
        {
            return await Task.FromResult(Ok(new { channelSlug, postSlug }));
        }

        [HttpGet("{channelSlug}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannel(string channelSlug)
        {
            async Task<IActionResult> channelFunction(Channel channel)
            {
                return await Task.FromResult(Ok(channel.ToChannelDto()));
            }

            return await UsingChannel(channelSlug, false, channelFunction);
        }

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostChannel([FromForm] NewChannel newChannel)
        {
            async Task<IActionResult> userFunction(User user)
            {
                var channelSlug = newChannel.CreateSlug();

                // Returns Conflict if channel slug already exists
                await new Add(channelSlug).ExecuteAsync();

                var channel = newChannel.Create(user);

                user.AddChannel(channel);

                await new Add(channel).ExecuteAsync();
                await new Update(user).ExecuteAsync();

                return Ok(user.ToUserEditDto());
            }

            return await UsingUser(userFunction);
        }
    }
}
