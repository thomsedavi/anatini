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
    [Route("api/spaces/{spaceHandle}/websites")]
    public class SpaceWebsitesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("{websiteHandle}/bookmark")]
        public async Task<IActionResult> PostWebsiteBookmark(string spaceHandle, string websiteHandle) => await UsingSpaceWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (work) =>
        {
            return await AddSpaceWebsiteEdge(Context, work.Id, UserWorkEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/bookmark")]
        public async Task<IActionResult> DeleteWebsiteBookmark(string spaceHandle, string websiteHandle) => await UsingUserWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/star")]
        public async Task<IActionResult> PostWebsiteStar(string spaceHandle, string websiteHandle) => await UsingUserWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddSpaceWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/star")]
        public async Task<IActionResult> DeleteWebsiteStar(string spaceHandle, string websiteHandle) => await UsingUserWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/dismiss")]
        public async Task<IActionResult> PostWebsiteDismiss(string spaceHandle, string websiteHandle) => await UsingUserWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddSpaceWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/dismiss")]
        public async Task<IActionResult> DeleteWebsiteDismiss(string spaceHandle, string websiteHandle) => await UsingUserWorkAsync(spaceHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddSpaceWebsiteEdge(ApplicationDbContext context, Guid websiteId, UserWorkEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWebsiteEdge = new ApplicationUserWorkEdge
                {
                    SourceUserId = sourceUserId,
                    TargetWorkId = websiteId,
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

        private async Task<IActionResult> DeleteUserWebsiteEdge(ApplicationDbContext context, Guid websiteId, UserWorkEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWebsiteEdge = await context.UserWorkEdges.FirstOrDefaultAsync(userWebsiteEdge => userWebsiteEdge.TargetWorkId == websiteId && userWebsiteEdge.SourceUserId == sourceUserId && userWebsiteEdge.Label == label);

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
