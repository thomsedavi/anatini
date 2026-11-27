using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Utils
{
    public static class ContentHelper
    {
        public static async Task<(bool? isBookmarked, bool? isDismissed, bool? isStarred, bool? isCollected, bool? isOwned, OwnerHeaderDto? ownerHeader)> GetUserContentDetails(Content content, Guid? userId, ApplicationUser? contentUser, Space? contentSpace, IBlobService? blobService = null)
        {
            bool? isBookmarked = null;
            bool? isDismissed = null;
            bool? isStarred = null;
            bool? isCollected = null;
            bool? isOwned = null;

            if (userId.HasValue)
            {
                isBookmarked = content.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked);
                isDismissed = content.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed);
                isStarred = content.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsStarred);
                isCollected = content.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsCollected);

                if (contentUser?.Id == userId.Value)
                {
                    isOwned = true;
                }
                else if (contentSpace?.UserRelationships.Any(userSpaceRelationship => userSpaceRelationship.Label == UserSpaceRelationshipLabel.IsOwned) ?? false)
                {
                    isOwned = true;
                }
                else
                {
                    isOwned = false;
                }
            }

            OwnerHeaderDto? ownerHeader = null;

            if (contentUser != null)
            {
                ownerHeader = await contentUser.ToOwnerHeaderDtoAsync(userId.HasValue, blobService);
            }
            else if (contentSpace != null)
            {
                ownerHeader = await contentSpace.ToOwnerHeaderDtoAsync(userId.HasValue, blobService);
            }

            return (isBookmarked, isDismissed, isStarred, isCollected, isOwned, ownerHeader);
        }
    }
}
