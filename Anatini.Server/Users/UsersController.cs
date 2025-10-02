using Anatini.Server.Context;
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
        public async Task<IActionResult> PostAlias([FromForm] NewUserAlias newUserAlias)
        {
            IActionResult userContextFunction(User user, AnatiniContext context)
            {
                if (user.Aliases.Count >= 5)
                {
                    return Forbid();
                }

                var userAlias = newUserAlias.Create(user);
                context.Add(userAlias);

                user.AddAlias(newUserAlias);
                context.Update(user);

                return Ok(user.ToUserEditDto());
            }

            return await UsingUserContext(userContextFunction);
        }

        [Authorize]
        [HttpGet("events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents()
        {
            async Task<IActionResult> userFunction(User user)
            {
                var userEvents = await new GetUserEvents(user.Id).ExecuteAsync();

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
                var userAliasResult = await new GetUserAlias(slug).ExecuteAsync();

                if (userAliasResult == null)
                {
                    return NotFound();
                }

                var userAlias = userAliasResult!;

                // TODO return 404 if slug requires authentication

                return Ok(userAlias.ToUserDto());
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
