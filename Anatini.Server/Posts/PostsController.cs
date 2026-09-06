using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts([FromQuery] PostsQuery query)
        {
            var nzNow = DateTime.UtcNow.ConvertUtcToNz();

            var postsQuery = Context.Posts.AsQueryable();

            postsQuery = postsQuery.AsNoTracking().Where(post => post.PublishedAtNz < nzNow);
            postsQuery = postsQuery.Include(post => post.User).ThenInclude(user => user!.Images);
            postsQuery = postsQuery.Include(post => post.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                postsQuery = postsQuery.Include(post => post.UserRelationships.Where(userPost => userPost.SourceUserId == sourceUserId));

                postsQuery = postsQuery.Where(post => (post.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasBookmarked));
                }

                if (query.Starred == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasStarred));
                }
                else if (query.Starred == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasStarred));
                }

                if (query.Dismissed == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasDismissed));
                }

                if (query.Collected == "only")
                {
                    postsQuery = postsQuery.Where(post => post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasCollected));
                }
                else if (query.Collected == "hide")
                {
                    postsQuery = postsQuery.Where(post => !post.UserRelationships.Any(userPost => userPost.SourceUserId == sourceUserId && userPost.Label == UserContentRelationshipLabel.HasCollected));
                }

                if (query.Followed == "only")
                {
                    postsQuery = postsQuery.Where(post => post.User != null && post.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.HasFollowed));
                }
                else if (query.Followed == "hide")
                {
                    postsQuery = postsQuery.Where(post => post.User != null && !post.User.ReceivedUserRelationships.Any(userRelationship => userRelationship.SourceUserId == sourceUserId && userRelationship.Label == UserUserRelationshipLabel.HasFollowed));
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

            return Ok(await Task.WhenAll(posts.Select(post => post.ToPostDtoAsync(IsAuthenticated, BlobService))));
        }
    }
}
