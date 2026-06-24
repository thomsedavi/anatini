using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/account/events")]
    public class AccountEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        public async Task<IActionResult> PostEvent([FromForm] CreateEvent createEvent) => await UsingAccountContextAsync(async (user, context) =>
        {
            var eventSeries = context.AddUserEventSeries(Visibility.Public, user.Id, createEvent.Name, createEvent.StartsAtNz);

            context.AddEventInstances(eventSeries);

            await context.SaveChangesAsync();

            return Ok();
        }, new ContextSettings { AccessRequired = true });
    }
}
