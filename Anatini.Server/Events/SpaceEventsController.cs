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
        public async Task<IActionResult> PostEvent(string spaceHandle, [FromForm] CreateEvent createEvent) => await UsingSpaceAsync(spaceHandle, async (space) =>
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

            var @event = Context.AddSpaceEvent(space.Id, createEvent, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published, validationResult.SanitizedHtml);

            Context.AddEventOccurrences(@event, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { eventId = @event.Id });
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{eventHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(string spaceHandle, string eventHandle) => await UsingSpaceContentAsync<Event>(spaceHandle, eventHandle, async (@event) =>
        {
            return Ok(@event.ToEventDto(IsAuthenticated));
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> PostEventOccurrenceBookmark(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> DeleteEventOccurrenceBookmark(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> PostEventOccurrenceStar(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> DeleteEventOccurrenceStar(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> PostEventOccurrenceDismiss(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> DeleteEventOccurrenceDismiss(string spaceHandle, string eventHandle, string eventOccurrenceHandle) => await UsingSpaceEventOccurrenceAsync(spaceHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteSpaceEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });

        private async Task<IActionResult> AddSpaceEventOccurrenceRelationship(ApplicationDbContext context, Guid eventOccurrenceId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventOccurrenceRelationship = new ApplicationUserContentRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = eventOccurrenceId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userEventOccurrenceRelationship);

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

        private async Task<IActionResult> DeleteSpaceEventOccurrenceRelationship(ApplicationDbContext context, Guid eventOccurrenceId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventOccurrenceRelationship = await context.UserContentRelationships.FirstOrDefaultAsync(userEventOccurrenceRelationship => userEventOccurrenceRelationship.TargetContentId == eventOccurrenceId && userEventOccurrenceRelationship.SourceUserId == sourceUserId && userEventOccurrenceRelationship.Label == label);

                if (userEventOccurrenceRelationship != null)
                {
                    context.Remove(userEventOccurrenceRelationship);
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
