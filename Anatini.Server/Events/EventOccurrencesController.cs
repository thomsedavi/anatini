using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
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
        public async Task<IActionResult> GetEventOccurrences([FromQuery] EventOccurrencesQuery query)
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var eventOccurrencesQuery = Context.EventOccurrences.AsQueryable();

            eventOccurrencesQuery = eventOccurrencesQuery
                .AsNoTracking()
                .Where(eventOccurrence => !eventOccurrence.PublishedAtNz.HasValue || eventOccurrence.PublishedAtNz.Value < nzNow)
                .Include(eventOccurrence => eventOccurrence.User).ThenInclude(user => user!.Images)
                .Include(eventOccurrence => eventOccurrence.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventOccurrencesQuery = eventOccurrencesQuery.Include(eventOccurrence => eventOccurrence.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => (eventOccurrence.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => !eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }

                if (query.Starred == "only")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }
                else if (query.Starred == "hide")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => !eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }

                if (query.Dismissed == "only")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => !eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }

                if (query.Collected == "only")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }
                else if (query.Collected == "hide")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => !eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }

                if (query.Followed == "only")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.User != null && eventOccurrence.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
                else if (query.Followed == "hide")
                {
                    eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.User != null && !eventOccurrence.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
            }
            else
            {
                eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.Visibility == Visibility.Public);
            }

            eventOccurrencesQuery = eventOccurrencesQuery.Where(eventOccurrence => eventOccurrence.StartsAtNz >= nzNow);

            var eventOccurrences = await eventOccurrencesQuery.OrderBy(eventOccurrence => eventOccurrence.StartsAtNz).ThenBy(eventOccurrence => eventOccurrence.Id).Take(10).ToListAsync();

            if (eventOccurrences == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventOccurrences.Select(eventOccurrence => eventOccurrence.ToEventOccurrenceDtoAsync(IsAuthenticated, BlobService))));
        }
    }
}
