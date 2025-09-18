using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
using Anatini.Server.Dtos;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelsController : AnatiniControllerBase
    {
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
                var newChannelSlug = NewChannelSlug.New(newChannel);
                var channelSlug = newChannelSlug.Create(newChannel);

                // Returns Conflict if channel slug already exists
                await new Add(channelSlug).ExecuteAsync();

                var channel = newChannel.Create(newChannelSlug, user);

                user.AddChannel(channel);

                await new Add(channel).ExecuteAsync();
                await new Update(user).ExecuteAsync();

                return Ok(new AccountDto(user));
            }

            return await UsingUser(userFunction);
        }
    }
}
