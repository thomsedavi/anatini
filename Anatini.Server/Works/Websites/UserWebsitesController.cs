using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
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
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostWebsite([FromForm] CreateWork createWork) => await UsingAccountAsync(async (user) =>
        {
            string? article = null;

            if (createWork.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createWork.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                article = validationResult.SanitizedHtml;
            }

            var website = Context.AddUserWorkAsync(WorkType.Product, createWork.Name, createWork.Url, createWork.Visibility, user.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle), article);

            await Context.SaveChangesAsync();

            website.User = user;

            return CreatedAtAction(nameof(GetWebsite), new { userHandle = user.Handle, websiteHandle = website.Handle }, await website.ToWorkDtoAsync(NormalizeHandleOrNull(createWork.Handle), BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{websiteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchWebsite(string userHandle, string websiteHandle, [FromForm] UpdateWork updateWork) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            if (updateWork.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updateWork.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                website.Article = validationResult.SanitizedHtml;
            }

            website.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(await website.ToWorkDtoAsync(websiteHandle, BlobService));
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [Authorize]
        [HttpGet("{websiteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWebsite(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return Ok(await website.ToWorkDtoAsync(websiteHandle, BlobService));
        });

        [Authorize]
        [HttpPost("{websiteHandle}/bookmark")]
        public async Task<IActionResult> PostWebsiteBookmark(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/bookmark")]
        public async Task<IActionResult> DeleteWebsiteBookmark(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/star")]
        public async Task<IActionResult> PostWebsiteStar(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/star")]
        public async Task<IActionResult> DeleteWebsiteStar(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{websiteHandle}/dismiss")]
        public async Task<IActionResult> PostWebsiteDismiss(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await AddUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{websiteHandle}/dismiss")]
        public async Task<IActionResult> DeleteWebsiteDismiss(string userHandle, string websiteHandle) => await UsingUserWorkAsync(userHandle, websiteHandle, WorkType.Website, async (website) =>
        {
            return await DeleteUserWebsiteEdge(Context, website.Id, UserWorkEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserWebsiteEdge(ApplicationDbContext context, Guid websiteId, UserWorkEdgeLabel label)
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
