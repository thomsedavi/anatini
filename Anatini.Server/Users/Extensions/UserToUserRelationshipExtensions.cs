using Anatini.Server.Context;
using Anatini.Server.Enums;

namespace Anatini.Server.Users.Extensions
{
    public static class UserToUserRelationshipExtensions
    {
        public static AnatiniContext AddUserToUserRelationships(this AnatiniContext context, Guid userId, Guid toUserId, params UserToUserRelationshipType[] relationshipTypes)
        {
            foreach (var relationshipType in relationshipTypes)
            {
                var userToUserRelationship = new UserToUserRelationship
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ToUserId = toUserId,
                    RelationshipType = Enum.GetName(relationshipType)!
                };

                context.Add(userToUserRelationship);
            }

            return context;
        }
    }
}
