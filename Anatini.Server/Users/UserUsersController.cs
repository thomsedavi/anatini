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
    [Route("api/users/{userHandle}")]
    public class UserUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize(Policy = "IsTrusted")]
        [HttpPost("trust")]
        public async Task<IActionResult> PostUserTrust(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return await AddUserUserRelationship(Context, user.Id, UserUserRelationshipLabel.HasTrusted);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpDelete("trust")]
        public async Task<IActionResult> DeleteUserTrust(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return await DeleteUserUserRelationship(Context, user.Id, UserUserRelationshipLabel.HasTrusted);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpPost("follow")]
        public async Task<IActionResult> PostUserFollow(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return await AddUserUserRelationship(Context, user.Id, UserUserRelationshipLabel.HasFollowed);
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpDelete("follow")]
        public async Task<IActionResult> DeleteUserFollow(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            return await DeleteUserUserRelationship(Context, user.Id, UserUserRelationshipLabel.HasFollowed);
        });

        private async Task<IActionResult> AddUserUserRelationship(ApplicationDbContext context, Guid targetUserId, UserUserRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                if (sourceUserId == targetUserId)
                {
                    return BadRequest();
                }

                var userUserRelationship = new ApplicationUserUserRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userUserRelationship);

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

        private async Task<IActionResult> DeleteUserUserRelationship(ApplicationDbContext context, Guid targetUserId, UserUserRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                if (sourceUserId == targetUserId)
                {
                    return BadRequest();
                }

                var userUserRelationship = await context.UserUserRelationships.FirstOrDefaultAsync(userUserRelationship => userUserRelationship.TargetUserId == targetUserId && userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == label);

                if (userUserRelationship != null)
                {
                    context.Remove(userUserRelationship);
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
