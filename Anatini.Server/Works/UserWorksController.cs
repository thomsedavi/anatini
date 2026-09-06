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
    [Route("api/users/{userHandle}/works")]
    public class UserWorksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostWork(string userHandle, [FromForm] CreateWork createWork) => await UsingUserAsync(userHandle, async (user) =>
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

            var work = Context.AddUserWorkAsync(createWork.Name, createWork.Visibility, user.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle), article, createWork.Url);

            await Context.SaveChangesAsync();

            work.User = user;

            return CreatedAtAction(nameof(GetWork), new { userHandle = user.Handle, workHandle = work.Handle }, await work.ToWorkDtoAsync(IsAuthenticated, BlobService));
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
        public async Task<IActionResult> PatchWork(string userHandle, string workHandle, [FromForm] UpdateWork updateWork) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
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
        public async Task<IActionResult> GetWork(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return Ok(await work.ToWorkDtoAsync(IsAuthenticated, BlobService));
        });

        [HttpGet]
        public async Task<IActionResult> GetWorks(string userHandle, [FromQuery] WorksQuery query) => await UsingUserAsync(userHandle, async (user) =>
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var worksQuery = Context.Works.Where(work => work.UserId == user.Id);

            worksQuery = worksQuery.AsNoTracking().Where(work => !work.PublishedAtNz.HasValue || work.PublishedAtNz.Value < nzNow);

            if (TryGetUserId(out Guid sourceUserId))
            {
                worksQuery = worksQuery.Include(work => work.UserRelationships.Where(userWork => userWork.SourceUserId == sourceUserId));

                worksQuery = worksQuery.Where(work => (work.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasBookmarked));
                }

                if (query.Starred == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasStarred));
                }
                else if (query.Starred == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasStarred));
                }

                if (query.Dismissed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasDismissed));
                }

                if (query.Collected == "only")
                {
                    worksQuery = worksQuery.Where(work => work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasCollected));
                }
                else if (query.Collected == "hide")
                {
                    worksQuery = worksQuery.Where(work => !work.UserRelationships.Any(userWork => userWork.SourceUserId == sourceUserId && userWork.Label == UserContentRelationshipLabel.HasCollected));
                }

                if (query.Followed == "only")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && work.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.HasFollowed));
                }
                else if (query.Followed == "hide")
                {
                    worksQuery = worksQuery.Where(work => work.User != null && !work.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.HasFollowed));
                }
            }
            else
            {
                worksQuery = worksQuery.Where(work => work.Visibility == Visibility.Public);
            }

            if (query.LastName != null && query.LastWorkId.HasValue)
            {
                worksQuery = worksQuery.Where(work => string.Compare(work.Name, query.LastName) > 0 || (work.Name == query.LastName && work.Id > query.LastWorkId.Value));
            }

            var works = await worksQuery.OrderBy(work => work.Name).ThenBy(work => work.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (works == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(works.Select(work => work.ToWorkDtoAsync(IsAuthenticated, BlobService))));
        });

        [Authorize]
        [HttpPost("{workHandle}/bookmark")]
        public async Task<IActionResult> PostWorkBookmark(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await AddUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{workHandle}/bookmark")]
        public async Task<IActionResult> DeleteWorkBookmark(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{workHandle}/star")]
        public async Task<IActionResult> PostWorkStar(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await AddUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{workHandle}/star")]
        public async Task<IActionResult> DeleteWorkStar(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{workHandle}/dismiss")]
        public async Task<IActionResult> PostWorkDismiss(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await AddUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{workHandle}/dismiss")]
        public async Task<IActionResult> DeleteWorkDismiss(string userHandle, string workHandle) => await UsingUserContentAsync<Work>(userHandle, workHandle, async (work) =>
        {
            return await DeleteUserWorkRelationship(Context, work.Id, UserContentRelationshipLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserWorkRelationship(ApplicationDbContext context, Guid workId, UserContentRelationshipLabel label)
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
