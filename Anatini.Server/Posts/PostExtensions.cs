using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts
{
    public static class PostExtensions
    {
        public static async Task<PostDto> ToPostDtoAsync(this Post post, Guid? userId, IBlobService? blobService = null, ApplicationUser? postUser = null, Space? postSpace = null)
        {
            postUser ??= post.User;
            postSpace ??= post.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned) = ContentHelper.GetUserContentRelationships(post, userId, postUser, postSpace);

            return new PostDto
            {
                Id = post.Id,
                UserHeader = postUser != null ? await postUser.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = postSpace != null ? await postSpace.ToSpaceHeaderDto(blobService) : null,
                Handle = post.Handle,
                Title = post.Title,
                Body = post.Body,
                Visibility = post.Visibility.ToString(),
                PublishedAtNz = post.PublishedAtNz ?? throw new InvalidOperationException(),
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
