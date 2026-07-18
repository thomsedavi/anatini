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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEvent([FromForm] CreateEvent createEvent) => await UsingAccountContextAsync(async (user) =>
        {
            var eventSeries = Context.AddUserEventSeries(user.Id, createEvent, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            Context.AddEventInstances(eventSeries, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { eventId = eventSeries.Id });
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{eventId}")]
        [Authorize(Policy = "IsTrusted")]
        public async Task<IActionResult> GetEvent(string eventId)
        {
            return Ok(eventId);
        }
    }
}
