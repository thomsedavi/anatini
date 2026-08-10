using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Works.Websites
{
    [ApiController]
    [Route("api/users/{userHandle}/websites")]
    public class UserWebsitesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("{websiteHandle}/bookmark")]
        public async Task<IActionResult> PostWebsiteBookmark(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/bookmark")]
        public async Task<IActionResult> DeleteWebsiteBookmark(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/star")]
        public async Task<IActionResult> PostWebsiteStar(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/star")]
        public async Task<IActionResult> DeleteWebsiteStar(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/dismiss")]
        public async Task<IActionResult> PostWebsiteDismiss(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/dismiss")]
        public async Task<IActionResult> DeleteWebsiteDismiss(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserPostEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserWebsiteEdge(ApplicationDbContext context, Guid websiteId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWebsiteEdge = new ApplicationUserPostEdge
                {
                    SourceUserId = sourceUserId,
                    TargetPostId = websiteId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userWebsiteEdge);

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

        private async Task<IActionResult> DeleteUserWebsiteEdge(ApplicationDbContext context, Guid websiteId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWebsiteEdge = await context.UserPostEdges.FirstOrDefaultAsync(userWebsiteEdge => userWebsiteEdge.TargetPostId == websiteId && userWebsiteEdge.SourceUserId == sourceUserId && userWebsiteEdge.Label == label);

                if (userWebsiteEdge != null)
                {
                    context.Remove(userWebsiteEdge);
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
