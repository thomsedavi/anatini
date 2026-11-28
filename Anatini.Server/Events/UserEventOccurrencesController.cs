using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/users/{userHandle}/event-occurrences")]
    public class UserEventOccurrencesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle, [FromQuery] EventOccurrencesQuery eventOccurrenceQuery) => await UsingUserAsync(userHandle, async (eventOccurrenceUser) =>
        {
            var utcNow = DateTime.UtcNow;

            var eventOccurrencesQueryable = Context.EventOccurrences.AsQueryable();

            eventOccurrencesQueryable = eventOccurrencesQueryable
                .AsNoTracking()
                .Where(eventOccurrence => eventOccurrence.UserId == eventOccurrenceUser.Id)
                .Where(eventOccurrence => !eventOccurrence.PublishedAtUtc.HasValue || eventOccurrence.PublishedAtUtc.Value < utcNow)
                .Include(eventOccurrence => eventOccurrence.Parent);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.Include(eventOccurrence => eventOccurrence.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                eventOccurrencesQueryable = eventOccurrencesQueryable.Where(eventOccurrence => (eventOccurrence.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                ApplyQueryFilters(eventOccurrencesQueryable, eventOccurrenceQuery, sourceUserId);
            }
            else
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.Where(eventOccurrence => eventOccurrence.Visibility == Visibility.Public);
            }

            eventOccurrencesQueryable = eventOccurrencesQueryable.Where(eventOccurrence => eventOccurrence.StartsAtUtc >= utcNow);

            var eventOccurrences = await eventOccurrencesQueryable.OrderBy(eventOccurrence => eventOccurrence.StartsAtUtc).ThenBy(eventOccurrence => eventOccurrence.Id).Take(10).ToListAsync();

            if (eventOccurrences == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventOccurrences.Select(eventOccurrence => eventOccurrence.ToEventOccurrenceDtoAsync(UserIdOrNull, BlobService, eventOccurrenceUser: eventOccurrenceUser))));
        });

        [Authorize]
        [HttpPost("{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> PostEventOccurrenceBookmark(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{eventOccurrenceHandle}/bookmark")]
        public async Task<IActionResult> DeleteEventOccurrenceBookmark(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{eventOccurrenceHandle}/like")]
        public async Task<IActionResult> PostEventOccurrenceLike(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsLiked);
        });

        [Authorize]
        [HttpDelete("{eventOccurrenceHandle}/like")]
        public async Task<IActionResult> DeleteEventOccurrenceLike(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsLiked);
        });

        [Authorize]
        [HttpPost("{eventOccurrenceHandle}/hide")]
        public async Task<IActionResult> PostEventOccurrenceHide(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await AddContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsHidden);
        });

        [Authorize]
        [HttpDelete("{eventOccurrenceHandle}/hide")]
        public async Task<IActionResult> DeleteEventOccurrenceHide(string userHandle, string eventOccurrenceHandle) => await UsingUserContentAsync<EventOccurrence>(userHandle, eventOccurrenceHandle, async (eventOccurrence, _) =>
        {
            return await DeleteContentRelationship(eventOccurrence.Id, UserContentRelationshipLabel.IsHidden);
        });
    }
}
