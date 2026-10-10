using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
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
                worksQuery = worksQuery.Include(work => work.UserRelationships.Where(userPost => userPost.SourceUserId == sourceUserId));

                worksQuery = worksQuery.Where(work => (work.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsBookmarked));
                }

                if (query.Starred == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsStarred));
                }
                else if (query.Starred == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsStarred));
                }

                if (query.Dismissed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsDismissed));
                }

                if (query.Collected == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsCollected));
                }
                else if (query.Collected == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.IsCollected));
                }

                if (query.Followed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && work.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
                else if (query.Followed == "hide")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && !work.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
            }
            else
            {
                worksQuery = worksQuery.Where(work => work.Visibility == Visibility.Public);
            }

            if (query.LastName != null && query.LastWorkId.HasValue)
            {
                worksQuery = worksQuery.Where(work => string.Compare(work.Name, query.LastName) > 0 || (work.Name == query.LastName && work.Id < query.LastWorkId.Value));
            }

            var works = await worksQuery.OrderBy(work => work.Name).ThenByDescending(work => work.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (works == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(works.Select(work => work.ToWorkDtoAsync(IsAuthenticated, BlobService))));
        }
    }
}
