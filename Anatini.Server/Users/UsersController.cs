using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IBlobService blobService) : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost("aliases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAlias([FromForm] NewUserAlias newUserAlias) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            if (user.Aliases.Count >= 5)
            {
                return Forbid();
            }

            var userAlias = await context.AddUserAliasAsync(user.Id, newUserAlias.Handle, user.Name, user.Protected, user.About);

            user.AddAlias(userAlias, newUserAlias.Default);
            await context.UpdateAsync(user);

            return Ok(await user.ToUserEditDto());
        });

        [Authorize]
        [HttpGet("events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents() => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            var userEvents = await context.Context.UserEvents.WithPartitionKey(user.Id).Where(userEvent => userEvent.UserId == user.Id).ToListAsync();

            return Ok(new { Events = userEvents.Select(userEvent => userEvent.ToUserEventDto()) });
        });

        [HttpGet("{handle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAlias(string handle) => await UsingContextAsync(async context =>
        {
            var userAlias = await context.FindAsync<UserAlias>(handle);

            if (userAlias == null)
            {
                return NotFound();
            }

            if (userAlias.Protected.HasValue && userAlias.Protected.Value && UserId != null && !await UserHasAnyPermission(UserId, UserPermission.Trusted, UserPermission.Admin))
            {
                return NotFound();
            }

            return Ok(await userAlias.ToUserDto(context, blobService));
        });

        [Authorize]
        [HttpPost("{toUserHandle}/relationships")]
        public async Task<IActionResult> PostRelationship(string toUserHandle, [FromForm] CreateRelationship createRelationship) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            string toUserId;

            if (RandomHex.IsX16(toUserHandle))
            {
                toUserId = toUserHandle;
            }
            else
            {
                var userAlias = await context.FindAsync<UserAlias>(toUserHandle) ?? throw new KeyNotFoundException();

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
                await context.UpdateAsync(user);
            }

            return await Task.FromResult(NoContent());
        }, UserPermission.Admin, UserPermission.Trusted);

        [Authorize]
        [HttpGet("{userId}/images/{imageId}")]
        public async Task<IActionResult> GetImage(string userId, string imageId) => await UsingUserAliasAsync(userId, async channelAlias =>
        {
            return await Task.FromResult(Ok($"TODO Image Result for {imageId}"));
        });
    }
}
