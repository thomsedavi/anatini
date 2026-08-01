using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet("{userHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return Ok(await user.ToUserDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { IncludeImages = true });

        [Authorize]
        [HttpGet("{userHandle}/images/{imageHandle}")]
        public async Task<IActionResult> GetImage(string userHandle, string imageHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return await Task.FromResult(Ok($"TODO Image Result for {imageHandle}"));
        });
    }
}
