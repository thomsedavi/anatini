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
        public async Task<IActionResult> PostWork(string spaceHandle, [FromForm] CreateWork createWork) => await UsingSpaceAsync(spaceHandle, async (workSpace) =>
        {
           var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createWork.Body);

           if (validationResult.ErrorMessage != null)
           {
               return BadRequest(new { error = validationResult.ErrorMessage });
           }
           else if (validationResult.SanitizedHtml == null)
           {
               return BadRequest(new { error = "Unknown error" });
           }

            var work = Context.AddSpaceWorkAsync(createWork.Title, validationResult.SanitizedHtml, createWork.Visibility, workSpace.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle));

            await Context.SaveChangesAsync();

            work.Space = workSpace;

            return CreatedAtAction(nameof(GetWork), new { spaceHandle = workSpace.Handle, workHandle = work.Handle }, await work.ToWorkDtoAsync(UserIdOrNull, BlobService, workSpace: workSpace));
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
        public async Task<IActionResult> PatchWork(string spaceHandle, string workHandle, [FromForm] UpdateWork updateWork) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, workSpace) =>
        {
            if (updateWork.Body != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updateWork.Body);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                work.Body = validationResult.SanitizedHtml;
            }

            work.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(await work.ToWorkDtoAsync(UserIdOrNull, BlobService, workSpace: workSpace));
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [Authorize]
        [HttpGet("{workHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWork(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, workSpace) =>
        {
            return Ok(await work.ToWorkDtoAsync(UserIdOrNull, BlobService, workSpace: workSpace));
        });

        [HttpGet]
        public async Task<IActionResult> GetWorks(string spaceHandle, [FromQuery] WorksQuery worksQuery) => await UsingSpaceAsync(spaceHandle, async (workSpace) =>
        {
            var utcNow = DateTime.UtcNow;

            var worksQueryable = Context.Works.Where(work => work.SpaceId == workSpace.Id);

            worksQueryable = worksQueryable.AsNoTracking().Where(work => !work.PublishedAtUtc.HasValue || work.PublishedAtUtc.Value < utcNow);

            if (TryGetUserId(out Guid sourceUserId))
            {
                worksQueryable = worksQueryable.Include(work => work.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                worksQueryable = worksQueryable.Where(work => (work.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                ApplyQueryFilters(worksQueryable, worksQuery, sourceUserId);
            }
            else
            {
                worksQueryable = worksQueryable.Where(work => work.Visibility == Visibility.Public);
            }

            if (worksQuery.LastTitle != null && worksQuery.LastWorkId.HasValue)
            {
                worksQueryable = worksQueryable.Where(work => string.Compare(work.Title, worksQuery.LastTitle) > 0 || (work.Title == worksQuery.LastTitle && work.Id > worksQuery.LastWorkId.Value));
            }

            var works = await worksQueryable.OrderBy(work => work.Title).ThenBy(work => work.Id).Take(worksQuery.PageSize ?? 10).ToListAsync();

            if (works == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(works.Select(work => work.ToWorkDtoAsync(UserIdOrNull, BlobService, workSpace: workSpace))));
        });

        [Authorize]
        [HttpPost("{workHandle}/bookmark")]
        public async Task<IActionResult> PostWorkBookmark(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{workHandle}/bookmark")]
        public async Task<IActionResult> DeleteWorkBookmark(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{workHandle}/star")]
        public async Task<IActionResult> PostWorkStar(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{workHandle}/star")]
        public async Task<IActionResult> DeleteWorkStar(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{workHandle}/dismiss")]
        public async Task<IActionResult> PostWorkDismiss(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{workHandle}/dismiss")]
        public async Task<IActionResult> DeleteWorkDismiss(string spaceHandle, string workHandle) => await UsingSpaceContentAsync<Work>(spaceHandle, workHandle, async (work, _) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsDismissed);
        });
    }
}
