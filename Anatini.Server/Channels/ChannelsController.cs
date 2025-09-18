using System.Net.Mime;
using Anatini.Server.Channels.Commands;
using Anatini.Server.Dtos;
using Anatini.Server.Users;
using Anatini.Server.Users.Commands;
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
        public async Task<IActionResult> PostChannel([FromForm] ChannelForm form)
        {
            async Task<IActionResult> userFunction(User user)
            {
                var channelId = Guid.NewGuid();
                var slugId = Guid.NewGuid();

                await new CreateChannelSlug(slugId, form.Slug, channelId, form.Name).ExecuteAsync();

                await new CreateChannel(channelId, form.Name, user.Id, user.Name, slugId, form.Slug).ExecuteAsync();

                user.AddChannel(channelId, form.Name);
                await new UpdateUser(user).ExecuteAsync();

                return Ok(new AccountDto(user));
            }

            return await UsingUser(userFunction);
        }
    }
}
