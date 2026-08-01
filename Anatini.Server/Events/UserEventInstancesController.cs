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
            return await AddUserEventInstanceEdge(Context, eventInstance.Id, UserEventInstanceEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("bookmark")]
        public async Task<IActionResult> DeleteEventInstanceBookmark(string userHandle, string eventSeriesHandle, string eventInstanceHandle) => await UsingUserEventInstanceAsync(userHandle, eventSeriesHandle, eventInstanceHandle, async (eventInstance) =>
        {
            return await DeleteUserEventInstanceEdge(Context, eventInstance.Id, UserEventInstanceEdgeLabel.HasBookmarked);
        });

        private async Task<IActionResult> AddUserEventInstanceEdge(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceEdge = new ApplicationUserEventInstanceEdge
                {
                    SourceUserId = sourceUserId,
                    TargetEventInstanceId = eventInstanceId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userEventInstanceEdge);

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

        private async Task<IActionResult> DeleteUserEventInstanceEdge(ApplicationDbContext context, Guid eventInstanceId, UserEventInstanceEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userEventInstanceEdge = await context.UserEventInstanceEdges.FirstOrDefaultAsync(userEventInstanceEdge => userEventInstanceEdge.TargetEventInstanceId == eventInstanceId && userEventInstanceEdge.SourceUserId == sourceUserId && userEventInstanceEdge.Label == label);

                if (userEventInstanceEdge != null)
                {
                    context.Remove(userEventInstanceEdge);
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
