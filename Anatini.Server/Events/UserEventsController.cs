using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/users/{userId}/events")]
    public class UserEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(string userId, string eventId) => await UsingUserEventAsync(userId, eventId, async (userEvent) =>
        {
            return Ok();
        });
    }
}
