using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Users
{
    [ApiController]
    [Route("api/users/{userId}")]
    public class UserUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize(Policy = "IsTrusted")]
        [HttpPost("trust")]
        public async Task<IActionResult> PostUserTrust(string userId) => await UsingUserAsync(userId, async (user) =>
        {
            return await AddUserUserEdge(Context, user.Id, UserUserEdgeLabel.HasTrusted);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpDelete("trust")]
        public async Task<IActionResult> DeleteUserTrust(string userId) => await UsingUserAsync(userId, async (user) =>
        {
            return await DeleteUserUserEdge(Context, user.Id, UserUserEdgeLabel.HasTrusted);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpPost("follow")]
        public async Task<IActionResult> PostUserFollow(string userId) => await UsingUserAsync(userId, async (user) =>
        {
            return await AddUserUserEdge(Context, user.Id, UserUserEdgeLabel.HasFollowed);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpDelete("follow")]
        public async Task<IActionResult> DeleteUserFollow(string userId) => await UsingUserAsync(userId, async (user) =>
        {
            return await DeleteUserUserEdge(Context, user.Id, UserUserEdgeLabel.HasFollowed);
        });

        private async Task<IActionResult> AddUserUserEdge(ApplicationDbContext context, Guid userId, UserUserEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userUserEdge = new ApplicationUserUserEdge
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = userId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userUserEdge);

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

        private async Task<IActionResult> DeleteUserUserEdge(ApplicationDbContext context, Guid userId, UserUserEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userUserEdge = await context.UserUserEdges.FirstOrDefaultAsync(userUserEdge => userUserEdge.TargetUserId == userId && userUserEdge.SourceUserId == sourceUserId && userUserEdge.Label == label);

                if (userUserEdge != null)
                {
                    context.Remove(userUserEdge);
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
