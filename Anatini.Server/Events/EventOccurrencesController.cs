using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/event-occurrences")]
    public class EventOccurrencesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences([FromQuery] EventOccurrencesQuery eventOccurrenceQuery)
        {
            var utcNow = DateTime.UtcNow;

            var eventOccurrencesQueryable = Context.EventOccurrences.AsQueryable();

            eventOccurrencesQueryable = eventOccurrencesQueryable
                .AsNoTracking()
                .Where(eventOccurrence => !eventOccurrence.PublishedAtUtc.HasValue || eventOccurrence.PublishedAtUtc.Value < utcNow)
                .Include(eventOccurrence => eventOccurrence.User).ThenInclude(user => user!.Images)
                .Include(eventOccurrence => eventOccurrence.Space).ThenInclude(space => space!.Images)
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

            return Ok(await Task.WhenAll(eventOccurrences.Select(eventOccurrence => eventOccurrence.ToEventOccurrenceDtoAsync(UserIdOrNull, BlobService))));
        }
    }
}
