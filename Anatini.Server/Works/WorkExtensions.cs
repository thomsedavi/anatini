using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Works
{
    public static class WorkExtensions
    {
        public static async Task<WorkDto> ToWorkDtoAsync(this Content work, bool isAuthenticated, IBlobService? blobService = null)
        {
            return new WorkDto
            {
                Id = work.Id,
                UserHeader = work.User != null ? await work.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = work.Space != null ? await work.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = work.Handle,
                Name = work.Name ?? throw new InvalidOperationException(),
                Article = work.Article,
                Url = work.Url,
                Visibility = work.Visibility.ToString(),
                PublishedAtNz = work.PublishedAtNz,
                HasBookmarked = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasBookmarked) : null,
                HasDismissed = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasDismissed) : null,
                HasStarred = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasStarred) : null,
                HasCollected = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.HasCollected) : null
            };
        }
    }
}
