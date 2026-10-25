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
    [Route("api/users/{userHandle}/event-occurrences")]
    public class UserEventOccurrencesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventOccurrences(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var eventInstancesQuery = Context.EventInstances.AsQueryable();

            eventInstancesQuery = eventInstancesQuery.AsNoTracking();

            eventInstancesQuery = eventInstancesQuery.Where(eventInstance => eventInstance.UserId == user.Id);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventInstancesQuery = eventInstancesQuery.Include(eventInstance => eventInstance.UserRelationships.Where(userRelationship => userRelationship.SourceUserId == sourceUserId));

                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => (eventInstance.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => eventInstance.Visibility == Visibility.Public);
            }

            eventInstancesQuery = eventInstancesQuery.Where(eventInstance => eventInstance.StartsAtNz >= nzNow);

            var eventInstances = await eventInstancesQuery.OrderBy(eventInstance => eventInstance.StartsAtNz).ThenBy(eventInstance => eventInstance.Id).Take(10).ToListAsync();

            if (eventInstances == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventInstances.Select(eventInstance => eventInstance.ToEventInstanceDtoAsync(IsAuthenticated, BlobService))));
        });
    }
}
