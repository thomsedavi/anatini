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
        public async Task<IActionResult> GetWorks([FromQuery] WorksQuery query)
        {
            var worksQuery = Context.Works.AsQueryable();

            worksQuery = worksQuery.Include(work => work.User).ThenInclude(user => user!.Images);
            worksQuery = worksQuery.Include(work => work.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                worksQuery = worksQuery.Include(work => work.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                worksQuery = worksQuery.Where(work => (work.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }

                if (query.Starred == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }
                else if (query.Starred == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }

                if (query.Dismissed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }
                else if (query.Dismissed != "show")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }

                if (query.Collected == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }
                else if (query.Collected == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }

                if (query.Followed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && work.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
                else if (query.Followed == "hide")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && !work.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
            }
            else
            {
                worksQuery = worksQuery.Where(work => work.Visibility == Visibility.Public);
            }

            if (query.LastTitle != null && query.LastWorkId.HasValue)
            {
                worksQuery = worksQuery.Where(work => string.Compare(work.Title, query.LastTitle) > 0 || (work.Title == query.LastTitle && work.Id < query.LastWorkId.Value));
            }

            var works = await worksQuery.OrderBy(work => work.Title).ThenByDescending(work => work.Id).Take(query.PageSize ?? 10).ToListAsync();

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
