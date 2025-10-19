using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : AnatiniControllerBase
    {
        [HttpGet("{channelId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannel(string channelId)
        {
            IActionResult channelFunction(Channel channel)
            {
                return Ok(channel.ToChannelDto());
            }

            return await UsingChannel(channelId, channelFunction);
        }

        [Authorize]
        [HttpGet("{channelId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannelEdit(string channelId)
        {
            IActionResult channelFunction(Channel channel)
            {
                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannel(channelId, channelFunction, requiresAuthorisation: true);
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
            async Task<IActionResult> userContextFunctionAsync(User user, AnatiniContext context)
            {
                await context.AddChannelAliasAsync(newChannel.Slug, newChannel.Id, newChannel.Name);
                var channel = await context.AddChannelAsync(newChannel.Id, newChannel.Name, newChannel.Slug, user.Id, user.Name);

                user.AddChannel(channel);
                await context.Update(user);

                return Ok(user.ToUserEditDto());
            }

            return await UsingUserContextAsync(UserId, userContextFunctionAsync);
        }
    }
}
