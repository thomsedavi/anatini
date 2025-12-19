using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
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

            var userAlias = await context.AddUserAliasAsync(user.Id, newUserAlias.Slug, user.Name, user.Protected);

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

        [HttpPost("{toUserSlug}/relationships")]
        public async Task<IActionResult> PostRelationship(string toUserSlug, [FromForm] CreateRelationship createRelationship) => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            if (!user.HasAnyPermission(UserPermission.Admin, UserPermission.Trusted))
            {
                return Forbid();
            }

            if (!Guid.TryParse(toUserSlug, out Guid toUserId))
            {
                var userAlias = await context.FindAsync<UserAlias>(toUserSlug) ?? throw new KeyNotFoundException();

                toUserId = userAlias.UserId;
            }

            if (user.Id == toUserId)
            {
                return BadRequest();
            }

            var toUser = await context.FindAsync<User>(toUserId) ?? throw new KeyNotFoundException();

            if (createRelationship.Type == "Trusts")
            {
                await context.AddUserToUserRelationshipsAsync(user.Id, toUser.Id, UserToUserRelationshipType.Trusts);
                await context.AddUserToUserRelationshipsAsync(toUser.Id, user.Id, UserToUserRelationshipType.TrustedBy);

                toUser.AddPermission(UserPermission.Trusted);
                await context.Update(user);
            }

            return await Task.FromResult(NoContent());
        });
    }
}
