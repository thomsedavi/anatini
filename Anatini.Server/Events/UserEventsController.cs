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
    [Route("api/users/{userHandle}/events")]
    public class UserEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet("{eventSeriesHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(string userHandle, string eventSeriesHandle) => await UsingUserEventAsync(userHandle, eventSeriesHandle, async (eventSeries) =>
        {
            return Ok(eventSeries.ToEventSeriesDto());
        });

        [HttpGet("{eventSeriesHandle}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle, string eventSeriesHandle, DateTime? lastStartsAtUtc, int pageSize = 20) => await UsingUserEventAsync(userHandle, eventSeriesHandle, async (eventSeries) =>
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

        [HttpGet("{eventSeriesHandle}/occurrence/{eventInstanceHandle}")]
        public async Task<IActionResult> GetEventOccurrence(string userHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingUserEventInstanceAsync(userHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return Ok(eventInstance.ToEventInstanceDto());
        });
    }
}
