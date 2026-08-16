using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Works
{
    [ApiController]
    [Route("api/users/{userHandle}/works")]
    public class UserWorksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        public async Task<IActionResult> GetWorks([FromQuery] WorksQuery query)
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var worksQuery = Context.Works.AsQueryable();

            worksQuery = worksQuery.AsNoTracking().Where(work => !work.PublishedAtNz.HasValue || work.PublishedAtNz.Value < nzNow);
            worksQuery = worksQuery.Include(work => work.User).ThenInclude(user => user!.Images);
            worksQuery = worksQuery.Include(work => work.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                worksQuery = worksQuery.Include(work => work.UserEdges.Where(userWork => userWork.SourceUserId == sourceUserId));

                worksQuery = worksQuery.Where(work => (work.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasBookmarked));
                }

                if (query.Starred == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasStarred));
                }
                else if (query.Starred == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasStarred));
                }

                if (query.Dismissed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserEdges.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserWorkEdgeLabel.HasDismissed));
                }

                if (query.Followed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && work.User.ReceivedUserEdges.Any(userEdge => userEdge.SourceUserId == sourceUserId && userEdge.Label == UserUserEdgeLabel.HasFollowed));
                }
                else if (query.Followed == "hide")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && !work.User.ReceivedUserEdges.Any(test => test.SourceUserId == sourceUserId && test.Label == UserUserEdgeLabel.HasFollowed));
                }
            }
            else
            {
                worksQuery = worksQuery.Where(work => work.Visibility == Visibility.Public);
            }

            if (query.LastName != null && query.LastWorkId.HasValue)
            {
                worksQuery = worksQuery.Where(note => string.Compare(note.Name, query.LastName) > 0 || (note.Name == query.LastName && note.Id > query.LastWorkId.Value));
            }

            var works = await worksQuery.OrderBy(work => work.Name).ThenBy(work => work.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (works == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(works.Select(work => work.ToWorkDtoAsync(work.Handle, BlobService))));
        }

        public class WorksQuery
        {
            public string? LastName { get; set; }
            public Guid? LastWorkId { get; set; }
            public int? PageSize { get; set; }
            public string? Bookmarked { get; set; }
            public string? Starred { get; set; }
            public string? Dismissed { get; set; }
            public string? Followed { get; set; }
        }
    }
}
