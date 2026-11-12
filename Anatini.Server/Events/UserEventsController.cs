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
    [Route("api/users/{userHandle}/events")]
    public class UserEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEvent(string userHandle, [FromForm] CreateEvent createEvent) => await UsingUserAsync(userHandle, async (user) =>
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

            var @event = Context.AddUserEvent(user.Id, createEvent, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published, validationResult.SanitizedHtml);

            Context.AddEventOccurrences(@event, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { eventId = @event.Id });
        }, new ContextSettings { AccessRequired = true });

        [DisableEndpoint]
        [HttpGet("{eventHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(string userHandle, string eventHandle) => await UsingUserContentAsync<Event>(userHandle, eventHandle, async (@event) =>
        {
            return Ok(@event.ToEventDto(IsAuthenticated));
        });

        [DisableEndpoint]
        [HttpGet("{eventHandle}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle, string eventHandle, DateTime? lastStartsAtUtc, int pageSize = 20) => await UsingUserContentAsync<Event>(userHandle, eventHandle, async (@event) =>
        {
            var eventOccurrencesQuery = Context.EventOccurrences.Where(eventOccurrence => eventOccurrence.EventId == @event.Id);

            if (IsAuthenticated)
            {
                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => (eventOccurrence.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.Visibility == Visibility.Public);
            }

            if (lastStartsAtUtc.HasValue)
            {
                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.StartsAtNz < lastStartsAtUtc.Value);
            }

            var eventOccurrences = await eventOccurrencesQuery.OrderBy(eventOccurrence => eventOccurrence.StartsAtNz).Take(pageSize).ToListAsync();

            if (eventOccurrences == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventOccurrences.Select(eventOccurrence => eventOccurrence.ToEventOccurrenceDtoAsync(IsAuthenticated))));
        });

        [DisableEndpoint]
        [HttpGet("{eventHandle}/occurrence/{eventOccurrenceHandle}")]
        public async Task<IActionResult> GetEventOccurrence(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return Ok(await eventOccurrence.ToEventOccurrenceDtoAsync(IsAuthenticated));
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrence/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> PostEventOccurrenceBookmark(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrence/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> DeleteEventOccurrenceBookmark(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrence/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> PostEventOccurrenceStar(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrence/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> DeleteEventOccurrenceStar(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrence/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> PostEventOccurrenceDismiss(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await AddUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrence/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> DeleteEventOccurrenceDismiss(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence) =>
        {
            return await DeleteUserEventOccurrenceRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });

        private async Task<IActionResult> AddUserEventOccurrenceRelationship(ApplicationDbContext context, Guid eventOccurrenceId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userContentRelationship = new ApplicationUserContentRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = eventOccurrenceId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userContentRelationship);

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

        private async Task<IActionResult> DeleteUserEventOccurrenceRelationship(ApplicationDbContext context, Guid eventOccurrenceId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userContentRelationship = await context.UserContentRelationships.FirstOrDefaultAsync(userContentRelationship => userContentRelationship.TargetContentId == eventOccurrenceId && userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == label);

                if (userContentRelationship != null)
                {
                    context.Remove(userContentRelationship);
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
