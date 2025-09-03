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
        [HttpPost("handles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostHandle([FromForm] HandleForm form)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await new GetUser(userId).ExecuteAsync();

                    var handles = (user.Handles ?? []).ToList();

                    if (handles.Count >= 5)
                    {
                        return Forbid();
                    }

                    var handleId = Guid.NewGuid();

                    await new CreateHandle(handleId, userId, form.Handle).ExecuteAsync();

                    var userHandle = new UserHandle
                    {
                        Id = handleId,
                        Handle = form.Handle
                    };

                    handles.Add(userHandle);
                    user.Handles = handles;

                    if (form.Default ?? false || !user.DefaultHandleId.HasValue)
                    {
                        user.DefaultHandleId = handleId;
                    }

                    await new UpdateUser(user).ExecuteAsync();

                    return Ok(new UserDto(user));
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
        public async Task<IActionResult> Events()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var userEvents = await new GetUserEvents(userId).ExecuteAsync();

                    return Ok(new { Events = userEvents.Select(userEvent => new UserEventDto(userEvent)) });
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

        [Authorize]
        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Account()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await new GetUser(userId).ExecuteAsync();

                    return Ok(new UserDto(user));
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
    }
}
