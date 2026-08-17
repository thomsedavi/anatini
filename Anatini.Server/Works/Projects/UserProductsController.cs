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

namespace Anatini.Server.Works.Projects
{
    [ApiController]
    [Route("api/users/{userHandle}/projects")]
    public class UserProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostProject([FromForm] CreateWork createWork) => await UsingAccountAsync(async (user) =>
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

            var project = Context.AddUserWorkAsync(WorkType.Project, createWork.Name, createWork.Url, createWork.Visibility, user.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle), article);

            await Context.SaveChangesAsync();

            project.User = user;

            return CreatedAtAction(nameof(GetProject), new { userHandle = user.Handle, projectHandle = project.Handle }, await project.ToWorkDtoAsync(NormalizeHandleOrNull(createWork.Handle), BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet("{projectHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProject(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return Ok(await project.ToWorkDtoAsync(projectHandle, BlobService));
        });

        [Authorize]
        [HttpPost("{projectHandle}/bookmark")]
        public async Task<IActionResult> PostProjectBookmark(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await AddUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{projectHandle}/bookmark")]
        public async Task<IActionResult> DeleteProjectBookmark(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await DeleteUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{projectHandle}/star")]
        public async Task<IActionResult> PostProjectStar(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await AddUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{projectHandle}/star")]
        public async Task<IActionResult> DeleteProjectStar(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await DeleteUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{projectHandle}/dismiss")]
        public async Task<IActionResult> PostProjectDismiss(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await AddUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{projectHandle}/dismiss")]
        public async Task<IActionResult> DeleteProjectDismiss(string userHandle, string projectHandle) => await UsingUserWorkAsync(userHandle, projectHandle, WorkType.Project, async (project) =>
        {
            return await DeleteUserProjectEdge(Context, project.Id, UserPostEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserProjectEdge(ApplicationDbContext context, Guid projectId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userProjectEdge = new ApplicationUserPostEdge
                {
                    SourceUserId = sourceUserId,
                    TargetPostId = projectId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userProjectEdge);

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

        private async Task<IActionResult> DeleteUserProjectEdge(ApplicationDbContext context, Guid projectId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userProjectEdge = await context.UserPostEdges.FirstOrDefaultAsync(userProjectEdge => userProjectEdge.TargetPostId == projectId && userProjectEdge.SourceUserId == sourceUserId && userProjectEdge.Label == label);

                if (userProjectEdge != null)
                {
                    context.Remove(userProjectEdge);
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
