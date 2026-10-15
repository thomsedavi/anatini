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
        public async Task<IActionResult> GetEventOccurrences()
        {
            var eventInstancesQuery = Context.EventInstances.AsQueryable();

            eventInstancesQuery = eventInstancesQuery.Include(eventInstance => eventInstance.User).ThenInclude(user => user!.Images);
            eventInstancesQuery = eventInstancesQuery.Include(eventInstance => eventInstance.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => (eventInstance.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                eventInstancesQuery = eventInstancesQuery.Where(eventInstance => eventInstance.Visibility == Visibility.Public);
            }

            var eventInstances = await eventInstancesQuery.ToListAsync();

            if (eventInstances == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(eventInstances.Select(eventInstance => eventInstance.ToEventInstanceDtoAsync(IsAuthenticated, BlobService))));
        }
    }
}
