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
        public static async Task<PostDto> ToPostDtoAsync(this Post post, bool isAuthenticated, IBlobService? blobService = null)
        {
            return new PostDto
            {
                Id = post.Id,
                UserHeader = post.User != null ? await post.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = post.Space != null ? await post.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = post.Handle,
                Header = post.Header,
                Article = post.Article,
                Visibility = post.Visibility.ToString(),
                PublishedAtNz = post.PublishedAtNz ?? throw new InvalidOperationException(),
                HasBookmarked = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasBookmarked) : null,
                HasDismissed = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasDismissed) : null,
                HasStarred = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasStarred) : null,
                HasCollected = isAuthenticated ? post.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasCollected) : null
            };
        }
    }
}
