using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager)
    {
        [Authorize]
        [HttpPost("aliases")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAlias([FromForm] NewUserAlias newUserAlias) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            return Ok();
        });

        [HttpGet("{handle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(string handle) => await UsingContextAsync(async context =>
        {
            ApplicationUser? user;

            var users = context.Users.AsNoTracking().Include(user => user.Images.Where(image => image.Handle == "icon")).AsQueryable();

            if (Guid.TryParse(handle, out Guid userId))
            {
                user = await users.FirstOrDefaultAsync(user => user.Id == userId);
            }
            else
            {
                var normalizedHandle = NormalizeHandle(handle);

                user = await users.FirstOrDefaultAsync(user => user.Handle == normalizedHandle || user.Handles.Any(userHandle => userHandle.Handle == normalizedHandle));
            }

            if (user == null)
            {
                return NotFound();
            }

            if (user.Visibility == Visibility.Public)
            {
                return Ok(await user.ToUserDto(blobService));
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (user.Visibility == Visibility.Protected)
            {
                return Ok(await user.ToUserDto(blobService));
            }

            // TODO handle private visibility
            return NotFound();
        });

        [Authorize]
        [HttpPost("{toUserHandle}/relationships")]
        public async Task<IActionResult> PostRelationship(string toUserHandle, [FromForm] CreateRelationship createRelationship) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            return Ok();
        });

        [Authorize]
        [HttpGet("{userId}/images/{imageId}")]
        public async Task<IActionResult> GetImage(string userId, string imageId) => await UsingUserAliasAsync(userId, async channelAlias =>
        {
            return await Task.FromResult(Ok($"TODO Image Result for {imageId}"));
        });
    }
}
