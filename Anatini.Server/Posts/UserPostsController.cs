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

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/users/{userHandle}/posts")]
    public class UserPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPost([FromForm] CreatePost createPost) => await UsingAccountAsync(async (user) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createPost.Article);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var post = Context.AddUserPostAsync(createPost.Name, validationResult.SanitizedHtml, createPost.Visibility, user.Id, Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createPost.Handle), createPost.PublishedAtNz);

            await Context.SaveChangesAsync();

            post.User = user;

            return CreatedAtAction(nameof(GetPost), new { userHandle = user.Handle, postHandle = post.Handle }, await post.ToPostDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts(DateTime? lastPublishedAtNz, Guid? lastPostId, int pageSize = 20) => await UsingAccountAsync(async (user) =>
        {
            var postsQuery = Context.Posts.AsQueryable();

            postsQuery = postsQuery.AsNoTracking();

            postsQuery = postsQuery.Where(post => post.UserId == user.Id);

            if (lastPublishedAtNz.HasValue && lastPostId.HasValue)
            {
                postsQuery = postsQuery.Where(post => post.PublishedAtNz < lastPublishedAtNz.Value || (post.PublishedAtNz == lastPublishedAtNz.Value && post.Id < lastPostId.Value));
            }

            var posts = await postsQuery.OrderByDescending(post => post.PublishedAtNz).ThenByDescending(post => post.Id).Take(pageSize).ToListAsync();

            if (posts == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(posts.Select(post => post.ToPostDtoAsync(IsAuthenticated, BlobService))));
        });

        [Authorize]
        [HttpPatch("{postHandle}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchPost(string userHandle, string postHandle, [FromForm] UpdatePost updatePost) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            if (updatePost.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updatePost.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                post.Article = validationResult.SanitizedHtml;
            }

            if (updatePost.PublishedAtNz.HasValue)
            {
                post.PublishedAtNz = updatePost.PublishedAtNz.Value;
            }

            post.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return NoContent();
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [Authorize]
        [HttpGet("{postHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string userHandle, string postHandle) => await UsingUserContentAsync<Post>(userHandle, postHandle, async (post) =>
        {
            return Ok(await post.ToPostDtoAsync(IsAuthenticated, BlobService));
        });

        [Authorize]
        [HttpPost("{postHandle}/bookmark")]
        public async Task<IActionResult> PostPostBookmark(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await AddUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{postHandle}/bookmark")]
        public async Task<IActionResult> DeletePostBookmark(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await DeleteUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{postHandle}/star")]
        public async Task<IActionResult> PostPostStar(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await AddUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{postHandle}/star")]
        public async Task<IActionResult> DeletePostStar(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await DeleteUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{postHandle}/dismiss")]
        public async Task<IActionResult> PostPostDismiss(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await AddUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{postHandle}/dismiss")]
        public async Task<IActionResult> DeletePostDismiss(string userHandle, string postHandle) => await UsingUserContentAsync<Content>(userHandle, postHandle, async (post) =>
        {
            return await DeleteUserPostRelationship(Context, post.Id, UserContentRelationshipLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserPostRelationship(ApplicationDbContext context, Guid postId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userPostRelationship = new ApplicationUserContentRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = postId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userPostRelationship);

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

        private async Task<IActionResult> DeleteUserPostRelationship(ApplicationDbContext context, Guid postId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userPostRelationship = await context.UserContentRelationships.FirstOrDefaultAsync(userPostRelationship => userPostRelationship.TargetContentId == postId && userPostRelationship.SourceUserId == sourceUserId && userPostRelationship.Label == label);

                if (userPostRelationship != null)
                {
                    context.Remove(userPostRelationship);
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
