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

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/spaces/{spaceHandle}/posts")]
    public class SpacePostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts(string spaceHandle, [FromQuery] PostsQuery query) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var postsQuery = Context.Posts.AsQueryable();

            postsQuery = postsQuery
                .AsNoTracking()
                .Where(post => post.SpaceId == space.Id)
                .Where(post => post.PublishedAtNz < nzNow);

            if (TryGetUserId(out Guid sourceUserId))
            {
                postsQuery = postsQuery.Include(post => post.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                postsQuery = postsQuery.Where(post => (post.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }

                if (query.Starred == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }
                else if (query.Starred == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }

                if (query.Dismissed == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }

                if (query.Collected == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }
                else if (query.Collected == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }

                if (query.Followed == "only")
                {
                    postsQuery = postsQuery.Where(post => post.User != null && post.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
                else if (query.Followed == "hide")
                {
                    postsQuery = postsQuery.Where(post => post.User != null && !post.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
            }
            else
            {
                postsQuery = postsQuery.Where(post => post.Visibility == Visibility.Public);
            }

            if (query.LastPublishedAtNz.HasValue && query.LastPostId.HasValue)
            {
                postsQuery = postsQuery.Where(post => post.PublishedAtNz < query.LastPublishedAtNz.Value || (post.PublishedAtNz == query.LastPublishedAtNz.Value && post.Id < query.LastPostId.Value));
            }

            var posts = await postsQuery.OrderByDescending(post => post.PublishedAtNz).ThenByDescending(post => post.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (posts == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(posts.Select(post => post.ToPostDtoAsync(IsAuthenticated, BlobService, space: space))));
        });

        [Authorize]
        [HttpPost("{postId}/bookmark")]
        public async Task<IActionResult> PostPostBookmark(string spaceHandle, string postId) => await UsingSpaceContentAsync<Post>(spaceHandle, postId, async (post) =>
        {
            return Ok();
        });

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPost(string spaceHandle, [FromForm] CreatePost createPost) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createPost.Body);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var post = Context.AddSpacePostAsync(createPost.Title, validationResult.SanitizedHtml, createPost.Visibility, space.Id, Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createPost.Handle));

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { spaceId = space.Id, postId = post.Id }, await post.ToPostDtoAsync(IsAuthenticated, BlobService, space: space));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{postHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchPost(string spaceHandle, string postHandle, [FromForm] UpdatePost updatePost) => await UsingSpaceContentAsync<Post>(spaceHandle, postHandle, async (post) =>
        {
            if (updatePost.Body != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updatePost.Body);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                post.Body = validationResult.SanitizedHtml;
            }

            if (updatePost.PublishedAtNz.HasValue)
            {
                post.PublishedAtNz = updatePost.PublishedAtNz.Value;
            }

            post.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(await post.ToPostDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [HttpGet("{postHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string spaceHandle, string postHandle) => await UsingSpaceContentAsync<Post>(spaceHandle, postHandle, async (post) =>
        {
            return Ok(await post.ToPostDtoAsync(IsAuthenticated, BlobService));
        });
    }
}
