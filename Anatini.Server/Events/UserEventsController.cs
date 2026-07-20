using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Events.Extensions;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/users/{userId}/events")]
    public class UserEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet("{eventSeriesId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(string userId, string eventSeriesId) => await UsingUserEventAsync(userId, eventSeriesId, async (eventSeries) =>
        {
            return Ok();
        });

        [HttpGet("{eventSeriesId}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userId, string eventSeriesId, DateTime? lastStartsAtUtc, int pageSize = 20) => await UsingUserEventAsync(userId, eventSeriesId, async (eventSeries) =>
        {
            var eventInstancesQuery = Context.EventInstances.Where(eventInstance => eventInstance.EventSeriesId == eventSeries.Id);

            if (IsAuthenticated)
            {
                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => (eventInstance.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => eventInstance.Visibility == Visibility.Public);
            }

            if (lastStartsAtUtc.HasValue)
            {
                eventInstancesQuery = eventInstancesQuery.Where(note => note.StartsAtNz < lastStartsAtUtc.Value);
            }

            var eventInstances = await eventInstancesQuery.OrderBy(eventInstance => eventInstance.StartsAtNz).Take(pageSize).ToListAsync();

            if (eventInstances == null)
            {
                return Problem();
            }

            return Ok(eventInstances.Select(eventInstance => eventInstance.ToEventInstanceDto()));
        });
    }
}
