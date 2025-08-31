using System.Security.Claims;
using Anatini.Server.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [Authorize]
        [HttpGet("settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPost()
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

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

    internal class UserDto(User user)
    {
        public string Name { get; } = user.Name;
        public IEnumerable<UserEmailDto> Emails { get; } = user.Emails.Select(email => new UserEmailDto(email));
        public IEnumerable<UserInviteDto>? Invites { get; } = user.Invites?.Select(invite => new UserInviteDto(invite));
    }

    internal class UserEmailDto(UserEmail email)
    {
        public string Email { get; } = email.Email;
        public bool Verified { get; } = email.Verified;
    }

    internal class UserInviteDto(UserInvite invite)
    {
        public string InviteCode { get; } = invite.InviteCode;
        public DateOnly CreatedDateNZ { get; } = invite.CreatedDateNZ;
        public bool Used { get; } = invite.Used;
    }
}
