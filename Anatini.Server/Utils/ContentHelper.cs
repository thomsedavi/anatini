using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;

namespace Anatini.Server.Utils
{
    public static class ContentHelper
    {
        public static (bool? isBookmarked, bool? isDismissed, bool? isStarred, bool? isCollected, bool? isOwned) GetUserContentRelationships(Content content, Guid? userId, ApplicationUser? contentUser, Space? contentSpace)
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

            return (isBookmarked, isDismissed, isStarred, isCollected, isOwned);
        }
    }
}
