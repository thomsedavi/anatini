using System.Net;
using System.Security.Claims;
using Anatini.Server.Commands;
using Anatini.Server.Dtos;
using Anatini.Server.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [Authorize]
        [HttpPost("slugs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSlug([FromForm] SlugForm form)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var userResult = await new GetUser(userId).ExecuteAsync();

                    if (userResult == null)
                    {
                        return Problem();
                    }

                    var user = userResult!;

                    if (user.Slugs.Count >= 5)
                    {
                        return Forbid();
                    }

                    var slugId = Guid.NewGuid();

                    await new CreateUserSlug(slugId, form.Slug, userId, user.Name).ExecuteAsync();

                    var userOwnedSlug = new UserOwnedSlug
                    {
                        SlugId = slugId,
                        UserId = userId,
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
                else
                {
                    return Problem();
                }
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                {
                    return Conflict();
                }
                else
                {
                    //logger.LogError(dbUpdateException, "Exception creating user");
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
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
