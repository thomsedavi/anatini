using System.Security.Claims;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost("slugs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSlug([FromForm] NewUserSlug newUserSlug)
        {
            async Task<IActionResult> userFunction(User user)
            {
                if (user.Slugs.Count >= 5)
                {
                    return Forbid();
                }

                var userSlug = newUserSlug.Create(user);

                await new Add(userSlug).ExecuteAsync();

                user.AddSlug(newUserSlug);
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
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var events = await new GetEvents(userId).ExecuteAsync();

                    return Ok(new { Events = events.Select(@event => @event.ToEventDto()) });
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpGet("{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSlug(string slug)
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
