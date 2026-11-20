using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Works
{
    [ApiController]
    [Route("api/works")]
    public class WorksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorks([FromQuery] WorksQuery worksQuery)
        {
            var worksQueryable = Context.Works.AsQueryable();

            worksQueryable = worksQueryable.Include(work => work.User).ThenInclude(user => user!.Images);
            worksQueryable = worksQueryable.Include(work => work.Space).ThenInclude(space => space!.Images);

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
                worksQueryable = worksQueryable.Where(work => string.Compare(work.Title, worksQuery.LastTitle) > 0 || (work.Title == worksQuery.LastTitle && work.Id < worksQuery.LastWorkId.Value));
            }

            var works = await worksQueryable.OrderBy(work => work.Title).ThenByDescending(work => work.Id).Take(worksQuery.PageSize ?? 10).ToListAsync();

            if (works == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(works.Select(work => work.ToWorkDtoAsync(UserIdOrNull, BlobService))));
        }

        [Authorize]
        [HttpPost("{workId:guid}/bookmark")]
        public async Task<IActionResult> PostWorkBookmark(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{workId:guid}/bookmark")]
        public async Task<IActionResult> DeleteWorkBookmark(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{workId:guid}/star")]
        public async Task<IActionResult> PostWorkStar(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{workId:guid}/star")]
        public async Task<IActionResult> DeleteWorkStar(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{workId:guid}/dismiss")]
        public async Task<IActionResult> PostWorkDismiss(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await AddContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{workId:guid}/dismiss")]
        public async Task<IActionResult> DeleteWorkDismiss(Guid workId) => await UsingContentAsync<Work>(workId, async (work) =>
        {
            return await DeleteContentRelationship(Context, work.Id, UserContentRelationshipLabel.IsDismissed);
        });
    }
}
