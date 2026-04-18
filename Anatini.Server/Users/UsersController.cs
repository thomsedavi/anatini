using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : AnatiniControllerBase(context, userManager)
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
        public async Task<IActionResult> GetAlias(string handle) => await UsingContextAsync(async context =>
        {
            return Ok();
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
