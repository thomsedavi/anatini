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
        [HttpGet("{eventSeriesId}")]
        public async Task<IActionResult> GetEvent(string userId, string eventSeriesId) => await UsingUserEventAsync(userId, eventSeriesId, async (eventSeries) =>
        {
            return Ok();
        });
    }
}
