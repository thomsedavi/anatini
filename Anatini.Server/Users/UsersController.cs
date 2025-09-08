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
                    var userResult = await new GetUser(userId).ExecuteAsync();

                    if (userResult == null)
                    {
                        return Problem();
                    }

                    var user = userResult!;

                    if (user.Handles.Count >= 5)
                    {
                        return Forbid();
                    }

                    var handleId = Guid.NewGuid();

                    await new CreateHandle(handleId, form.Handle, userId, user.Name).ExecuteAsync();

                    var userHandle = new UserHandle
                    {
                        HandleId = handleId,
                        UserId = userId,
                        Value = form.Handle
                    };

                    user.Handles.Add(userHandle);

                    if (form.Default ?? false)
                    {
                        user.DefaultHandleId = handleId;
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

        [HttpGet("{handleValue}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHandle(string handleValue)
        {
            try
            {
                var handleResult = await new GetHandle(handleValue).ExecuteAsync();

                if (handleResult == null)
                {
                    return NotFound();
                }

                var handle = handleResult!;

                // TODO return 404 if handle requires authentication

                return Ok(new HandleDto(handle));
            }
            catch (Exception)
            {
                return Problem();
            }
        }
    }
}
