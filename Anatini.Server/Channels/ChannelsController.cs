using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Channels.Queries;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
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
                var channelSlug = newChannel.CreateSlug();

                // Returns Conflict if channel slug already exists
                await new Add(channelSlug).ExecuteAsync();

                var channel = newChannel.Create(user);

                user.AddChannel(channel);

                await new Add(channel).ExecuteAsync();
                await new Update(user).ExecuteAsync();

                return Ok(user.ToAccountDto());
            }

            return await UsingUser(userFunction);
        }

        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSlug(string slug)
        {
            try
            {
                var channelSlugResult = await new GetChannelSlug(slug).ExecuteAsync();

                if (channelSlugResult == null)
                {
                    return NotFound();
                }

                // TODO Channel slug should also contain eight recent posts, shouldn't need to retrieve channel
                var channelSlug = channelSlugResult!;

                var channelResult = await new GetChannel(channelSlug.ChannelId).ExecuteAsync();

                if (channelResult == null)
                {
                    return NotFound();
                }

                var channel = channelResult!;

                // TODO return 404 if slug requires authentication

                return Ok(channel.ToChannelDto());
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
