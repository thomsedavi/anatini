using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Posts.Links
{
    [ApiController]
    [Route("api/users/{userHandle}/links")]
    public class UserLinksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("{linkHandle}/bookmark")]
        public async Task<IActionResult> PostLinkBookmark(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await AddUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{linkHandle}/bookmark")]
        public async Task<IActionResult> DeleteLinkBookmark(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await DeleteUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{linkHandle}/star")]
        public async Task<IActionResult> PostLinkStar(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await AddUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{linkHandle}/star")]
        public async Task<IActionResult> DeleteLinkStar(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await DeleteUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{linkHandle}/dismiss")]
        public async Task<IActionResult> PostLinkDismiss(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await AddUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{linkHandle}/dismiss")]
        public async Task<IActionResult> DeleteLinkDismiss(string userHandle, string linkHandle) => await UsingUserPostAsync(userHandle, linkHandle, PostType.Link, async (link) =>
        {
            return await DeleteUserLinkEdge(Context, link.Id, UserPostEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserLinkEdge(ApplicationDbContext context, Guid linkId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userLinkEdge = new ApplicationUserPostEdge
                {
                    SourceUserId = sourceUserId,
                    TargetPostId = linkId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userLinkEdge);

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

        private async Task<IActionResult> DeleteUserLinkEdge(ApplicationDbContext context, Guid linkId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userLinkEdge = await context.UserPostEdges.FirstOrDefaultAsync(userLinkEdge => userLinkEdge.TargetPostId == linkId && userLinkEdge.SourceUserId == sourceUserId && userLinkEdge.Label == label);

                if (userLinkEdge != null)
                {
                    context.Remove(userLinkEdge);
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
