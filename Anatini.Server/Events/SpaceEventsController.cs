using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/spaces/{spaceHandle}/events")]
    public class SpaceEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEventSeries(string spaceHandle, [FromForm] CreateEvent createEvent) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createEvent.Body);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var eventSeries = Context.AddSpaceEventSeries(space.Id, createEvent, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published, validationResult.SanitizedHtml);

            Context.AddEventInstances(eventSeries, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventSeries), new { eventId = eventSeries.Id });
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{eventSeriesHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventSeries(string spaceHandle, string eventSeriesHandle) => await UsingSpaceContentAsync<EventSeries>(spaceHandle, eventSeriesHandle, async (eventSeries) =>
        {
            return Ok(eventSeries.ToEventDto(IsAuthenticated));
        });
    }
}
