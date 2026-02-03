using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserToUserRelationshipContextExtensions
    {
        public static async Task<AnatiniContext> AddUserToUserRelationshipsAsync(this AnatiniContext context, string userId, string toUserId, params UserToUserRelationshipType[] relationshipTypes)
        {
            foreach (var relationshipType in relationshipTypes)
            {
                var userToUserRelationship = new UserToUserRelationship
                {
                    ItemId = ItemId.Get(userId, Enum.GetName(relationshipType)!, toUserId),
                    UserId = userId,
                    RelationshipType = Enum.GetName(relationshipType)!,
                    ToUserId = toUserId
                };

                await context.AddAsync(userToUserRelationship);
            }

            return context;
        }
    }
}
