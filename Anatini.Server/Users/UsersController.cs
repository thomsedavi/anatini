using System.Security.Claims;
using Anatini.Server.Commands;
using Anatini.Server.Dtos;
using Anatini.Server.Queries;
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
        public async Task<IActionResult> PostSlug([FromForm] SlugForm form)
        {
            async Task<IActionResult> userFunction(User user)
            {
                if (user.Slugs.Count >= 5)
                {
                    return Forbid();
                }

                var slugId = Guid.NewGuid();

                await new CreateUserSlug(slugId, form.Slug, user.Id, user.Name).ExecuteAsync();

                var userOwnedSlug = new UserOwnedSlug
                {
                    SlugId = slugId,
                    UserId = user.Id,
                    Slug = form.Slug
                };

                user.Slugs.Add(userOwnedSlug);

                if (form.Default ?? false)
                {
                    user.DefaultSlugId = slugId;
                }

                await new UpdateUser(user).ExecuteAsync();

                return Ok(new AccountDto(user));
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

                    return Ok(new { Events = events.Select(@event => new EventDto(@event)) });
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

                return Ok(new UserSlugDto(userSlug));
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
