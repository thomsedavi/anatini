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
        public async Task<IActionResult> PostEventSeries(string userHandle, [FromForm] CreateEvent createEvent) => await UsingUserAsync(userHandle, async (user) =>
        {
            string? article = null;

            if (createEvent.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createEvent.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                article = validationResult.SanitizedHtml;
            }

            var eventSeries = Context.AddUserEventSeries(user.Id, createEvent, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published, article);

            Context.AddEventInstances(eventSeries, (createEvent.IsDraft ?? false) ? Status.Draft : Status.Published);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventSeries), new { eventId = eventSeries.Id });
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{eventSeriesHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventSeries(string userHandle, string eventSeriesHandle) => await UsingUserContentAsync<EventSeries>(userHandle, eventSeriesHandle, async (eventSeries) =>
        {
            return Ok(eventSeries.ToEventSeriesDto());
        });

        [HttpGet("{eventSeriesHandle}/occurrences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle, string eventSeriesHandle, DateTime? lastStartsAtUtc, int pageSize = 20) => await UsingUserContentAsync<EventSeries>(userHandle, eventSeriesHandle, async (eventSeries) =>
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

            return Ok(eventInstances.Select(eventInstance => eventInstance.ToEventInstanceDto(IsAuthenticated)));
        });

        [HttpGet("{eventSeriesHandle}/occurrence/{eventInstanceHandle}")]
        public async Task<IActionResult> GetEventOccurrence(string userHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingUserEventInstanceAsync(userHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return Ok(eventInstance.ToEventInstanceDto(IsAuthenticated));
        });
    }
}
