using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> PostAlias([FromForm] NewUserAlias newUserAlias) => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            if (user.Aliases.Count >= 5)
            {
                return Forbid();
            }

            var userAlias = await context.AddUserAliasAsync(user.Id, newUserAlias.Slug, user.Name);

            user.AddAlias(userAlias, newUserAlias.Default);
            await context.Update(user);

            return Ok(user.ToUserEditDto());
        });

        [Authorize]
        [HttpGet("events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents() => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            var userEvents = await context.Context.UserEvents.WithPartitionKey(user.Id).Where(userEvent => userEvent.UserId == user.Id).ToListAsync();

            return Ok(new { Events = userEvents.Select(userEvent => userEvent.ToUserEventDto()) });
        });

        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAlias(string slug) => await UsingContextAsync(async context =>
        {
            var userAlias = await context.FindAsync<UserAlias>(slug);

            if (userAlias == null)
            {
                return NotFound();
            }

            // TODO return 404 if slug requires authentication

            return Ok(userAlias.ToUserDto());
        });
    }
}
