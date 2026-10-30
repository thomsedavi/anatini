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
        public static async Task<WorkDto> ToWorkDtoAsync(this Content work, bool isAuthenticated, IBlobService? blobService = null, ApplicationUser? user = null, Space? space = null)
        {
            user ??= work.User;
            space ??= work.Space;

            return new WorkDto
            {
                Id = work.Id,
                UserHeader = user != null ? await user.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = space != null ? await space.ToSpaceHeaderDto(blobService) : null,
                Handle = work.Handle,
                Title = work.Title ?? throw new InvalidOperationException(),
                Body = work.Body,
                Visibility = work.Visibility.ToString(),
                PublishedAtNz = work.PublishedAtNz,
                IsBookmarked = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsBookmarked) : null,
                IsDismissed = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsDismissed) : null,
                IsStarred = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsStarred) : null,
                IsCollected = isAuthenticated ? work.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsCollected) : null
            };
        }
    }
}
