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

            return CreatedAtAction(nameof(GetEvent), new {userHandle = user.Handle, eventHandle = @event.Handle }, @event.ToEventDto(UserIdOrNull, eventUser: user));
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{eventHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(string userHandle, string eventHandle) => await UsingUserContentAsync<Event>(userHandle, eventHandle, async (@event, eventUser) =>
        {
            return Ok(@event.ToEventDto(UserIdOrNull, eventUser: eventUser));
        });

        [HttpGet("{eventHandle}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle, string eventHandle, DateTime? lastStartsAtUtc, int pageSize = 20) => await UsingUserContentAsync<Event>(userHandle, eventHandle, async (@event, eventOccurrenceUser) =>
        {
            var eventOccurrencesQuery = Context.EventOccurrences.Where(eventOccurrence => eventOccurrence.ParentId == @event.Id);

            eventOccurrencesQuery.Include(eventOccurrence => eventOccurrence.Parent);

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
                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.StartsAtUtc < lastStartsAtUtc.Value);
            }

            var eventOccurrences = await eventOccurrencesQuery.OrderBy(eventOccurrence => eventOccurrence.StartsAtUtc).Take(pageSize).ToListAsync();

            if (eventOccurrences == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventOccurrences.Select(eventOccurrence => eventOccurrence.ToEventOccurrenceDtoAsync(UserIdOrNull, eventOccurrenceUser: eventOccurrenceUser))));
        });

        [HttpGet("{eventHandle}/occurrences/{eventOccurrenceHandle}")]
        public async Task<IActionResult> GetEventOccurrence(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, eventOccurrenceUser) =>
        {
            return Ok(await eventOccurrence.ToEventOccurrenceDtoAsync(UserIdOrNull, eventOccurrenceUser: eventOccurrenceUser));
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> PostEventOccurrenceBookmark(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> DeleteEventOccurrenceBookmark(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> PostEventOccurrenceStar(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/star")]
        public async Task<IActionResult> DeleteEventOccurrenceStar(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{eventHandle}/occurrences/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> PostEventOccurrenceDismiss(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{eventHandle}/occurrences/{eventOccurrenceHandle}/dismiss")]
        public async Task<IActionResult> DeleteEventOccurrenceDismiss(string userHandle, string eventHandle, string eventOccurrenceHandle) => await UsingUserEventOccurrenceAsync(userHandle, eventHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(Context, eventOccurrence.Id, UserContentRelationshipLabel.IsDismissed);
        });
    }
}
