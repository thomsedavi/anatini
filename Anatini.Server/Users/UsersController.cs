using Anatini.Server.Context.Commands;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User = Anatini.Server.Context.User;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost("aliases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSlug([FromForm] NewUserAlias newUserAlias)
        {
            async Task<IActionResult> userFunction(User user)
            {
                if (user.Aliases.Count >= 5)
                {
                    return Forbid();
                }

                var userAlias = newUserAlias.Create(user);

                await new Add(userAlias).ExecuteAsync();

                user.AddAlias(newUserAlias);
                await new Update(user).ExecuteAsync();

                return Ok(user.ToUserEditDto());
            }

            return await UsingUser(userFunction);
        }

        [Authorize]
        [HttpGet("events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents()
        {
            async Task<IActionResult> userFunction(User user)
            {
                var userEvents = await new GetUserEvents(user.Guid).ExecuteAsync();

                return Ok(new { Events = userEvents.Select(userEvent => userEvent.ToUserEventDto()) });
            }

            return await UsingUser(userFunction);
        }

        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAlias(string slug)
        {
            try
            {
                var userSlugResult = await new GetUserSlug(slug).ExecuteAsync();

                if (userSlugResult == null)
                {
                    return NotFound();
                }

                var userSlug = userSlugResult!;

                // TODO return 404 if slug requires authentication

                return Ok(userSlug.ToUserDto());
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
