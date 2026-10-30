using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Posts
{
    public static class PostExtensions
    {
        public static async Task<PostDto> ToPostDtoAsync(this Post post, bool isAuthenticated, IBlobService? blobService = null, ApplicationUser? user = null, Space? space = null)
        {
            user ??= post.User;
            space ??= post.Space;

            return new PostDto
            {
                Id = post.Id,
                UserHeader = user != null ? await user.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = space != null ? await space.ToSpaceHeaderDto(blobService) : null,
                Handle = post.Handle,
                Title = post.Title,
                Body = post.Body,
                Visibility = post.Visibility.ToString(),
                PublishedAtNz = post.PublishedAtNz ?? throw new InvalidOperationException(),
                IsBookmarked = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsBookmarked) : null,
                IsDismissed = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsDismissed) : null,
                IsStarred = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsStarred) : null,
                IsCollected = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsCollected) : null
            };
        }
    }
}
