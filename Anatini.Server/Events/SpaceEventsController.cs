using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        [Authorize]
        [HttpPost("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/bookmark")]
        public async Task<IActionResult> PostEventOccurrenceBookmark(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await AddSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/bookmark")]
        public async Task<IActionResult> DeleteEventOccurrenceBookmark(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await DeleteSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/star")]
        public async Task<IActionResult> PostEventOccurrenceStar(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await AddSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/star")]
        public async Task<IActionResult> DeleteEventOccurrenceStar(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await DeleteSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/dismiss")]
        public async Task<IActionResult> PostEventOccurrenceDismiss(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await AddSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{eventSeriesHandle}/occurrence/{eventInstanceHandle}/dismiss")]
        public async Task<IActionResult> DeleteEventOccurrenceDismiss(string spaceHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingSpaceEventInstanceAsync(spaceHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await DeleteSpaceEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.IsDismissed);
        });

        private async Task<IActionResult> AddSpaceEventInstanceRelationship(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceRelationship = new ApplicationUserEventInstanceRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetEventInstanceId = eventInstanceId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userEventInstanceRelationship);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                }

                return Created();
            }
            else
            {
                return Problem();
            }
        }

        private async Task<IActionResult> DeleteSpaceEventInstanceRelationship(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceRelationship = await context.UserEventInstanceRelationships.FirstOrDefaultAsync(userEventInstanceRelationship => userEventInstanceRelationship.TargetEventInstanceId == eventInstanceId && userEventInstanceRelationship.SourceUserId == sourceUserId && userEventInstanceRelationship.Label == label);

                if (userEventInstanceRelationship != null)
                {
                    context.Remove(userEventInstanceRelationship);
                    await context.SaveChangesAsync();
                }

                return NoContent();
            }
            else
            {
                return Problem();
            }
        }
    }
}
