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

namespace Anatini.Server.Works
{
    [ApiController]
    [Route("api/spaces/{spaceHandle}/works")]
    public class SpaceWorksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostWork(string spaceHandle, [FromForm] CreateWork createWork) => await UsingSpaceAsync(spaceHandle, async (space) =>
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

            var work = Context.AddSpaceWorkAsync(createWork.Name, createWork.Visibility, space.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle), article, createWork.Url);

            await Context.SaveChangesAsync();

            work.Space = space;

            return CreatedAtAction(nameof(GetWork), new { spaceHandle = space.Handle, workHandle = work.Handle }, await work.ToWorkDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{workHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchWork(string spaceHandle, string workHandle, [FromForm] UpdateWork updateWork) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
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

                work.Article = validationResult.SanitizedHtml;
            }

            if (updateWork.Url != null)
            {
                work.Url = updateWork.Url;
            }

            work.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(await work.ToWorkDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [Authorize]
        [HttpGet("{workHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWork(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return Ok(await work.ToWorkDtoAsync(IsAuthenticated, BlobService));
        });

        [Authorize]
        [HttpPost("{workHandle}/bookmark")]
        public async Task<IActionResult> PostWorkBookmark(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await AddSpaceWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{workHandle}/bookmark")]
        public async Task<IActionResult> DeleteWorkBookmark(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{workHandle}/star")]
        public async Task<IActionResult> PostWorkStar(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await AddSpaceWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{workHandle}/star")]
        public async Task<IActionResult> DeleteWorkStar(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{workHandle}/dismiss")]
        public async Task<IActionResult> PostWorkDismiss(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await AddSpaceWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{workHandle}/dismiss")]
        public async Task<IActionResult> DeleteWorkDismiss(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasDismissed);
        });

        private async Task<IActionResult> AddSpaceWorkRelationship(ApplicationDbContext context, Guid workId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWorkRelationship = new ApplicationUserContentRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = workId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userWorkRelationship);

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

        private async Task<IActionResult> DeleteUserWorkRelationship(ApplicationDbContext context, Guid workId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userWorkRelationship = await context.UserContentRelationships.FirstOrDefaultAsync(userWorkRelationship => userWorkRelationship.TargetContentId == workId && userWorkRelationship.SourceUserId == sourceUserId && userWorkRelationship.Label == label);

                if (userWorkRelationship != null)
                {
                    context.Remove(userWorkRelationship);
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
