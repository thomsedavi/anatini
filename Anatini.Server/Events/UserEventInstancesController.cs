using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Events
{
    [ApiController]
    [Route("api/users/{userHandle}/events/{eventSeriesHandle}/instances/{eventInstanceHandle}")]
    public class UserEventInstancesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("bookmark")]
        public async Task<IActionResult> PostEventInstanceBookmark(string userHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingUserEventInstanceAsync(userHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await AddUserEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("bookmark")]
        public async Task<IActionResult> DeleteEventInstanceBookmark(string userHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingUserEventInstanceAsync(userHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await DeleteUserEventInstanceRelationship(Context, eventInstance.Id, UserEventInstanceRelationshipLabel.HasBookmarked);
        });

        private async Task<IActionResult> AddUserEventInstanceRelationship(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceRelationship = new ApplicationUserEventInstanceRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetEventInstanceId = eventInstanceId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userEventInstanceRelationship);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                }

                return Created();
            }
            else
            {
                return Problem();
            }
        }

        private async Task<IActionResult> DeleteUserEventInstanceRelationship(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceRelationship = await context.UserEventInstanceRelationships.FirstOrDefaultAsync(userEventInstanceRelationship => userEventInstanceRelationship.TargetEventInstanceId == eventInstanceId && userEventInstanceRelationship.SourceUserId == sourceUserId && userEventInstanceRelationship.Label == label);

                if (userEventInstanceRelationship != null)
                {
                    context.Remove(userEventInstanceRelationship);
                    await context.SaveChangesAsync();
                }

                return NoContent();
            }
            else
            {
                return Problem();
            }
        }
    }
}
