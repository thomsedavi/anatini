using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/spaces/{spaceHandle}/events")]
    public class SpaceEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostEventSeries(string spaceHandle, [FromForm] CreateEvent createEvent) => await UsingSpaceAsync(spaceHandle, async (space) =>
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

            var eventSeries = Context.AddSpaceEventSeries(space.Id, createEvent, validationResult.SanitizedHtml);

            Context.AddEventInstances(eventSeries, Status.Published);

            await Context.SaveChangesAsync();

            return Ok(createEvent.Title);
        }, new ContextSettings { AccessRequired = true });
    }
}
