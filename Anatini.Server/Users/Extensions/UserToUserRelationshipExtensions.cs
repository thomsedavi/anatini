using Anatini.Server.Context;
using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Users.Extensions
{
    public static class UserToUserRelationshipExtensions
    {
        public static AnatiniContext AddUserToUserRelationships(this AnatiniContext context, string userId, string toUserId, params UserToUserRelationshipType[] relationshipTypes)
        {
            foreach (var relationshipType in relationshipTypes)
            {
                var userToUserRelationship = new UserToUserRelationship
                {
                    Id = IdGenerator.Get(),
                    UserId = userId,
                    RelationshipType = Enum.GetName(relationshipType)!,
                    ToUserId = toUserId
                };

                context.Add(userToUserRelationship);
            }

            return context;
        }
    }
}
