using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : AnatiniControllerBase
    {
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
        [HttpGet("{channelSlug}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannelEdit(string channelSlug)
        {
            async Task<IActionResult> channelFunction(Channel channel)
            {
                return await Task.FromResult(Ok(channel.ToChannelEditDto()));
            }

            return await UsingChannel(channelSlug, true, channelFunction);
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
            IActionResult userContextFunction(User user, AnatiniContext context)
            {
                var channel = newChannel.Create(user);
                var channelAlias = newChannel.CreateAlias();
                context.AddRange(channel, channelAlias);

                user.AddChannel(channel);
                context.Update(user);

                return Ok(user.ToUserEditDto());
            }

            return await UsingUserContext(userContextFunction);
        }
    }
}
