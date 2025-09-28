using Anatini.Server.Context;
using Anatini.Server.Enums;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateUserToUserRelationships(Guid userId, Guid toUserId, params UserToUserRelationshipType[] relationshipTypes) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

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

            return await context.SaveChangesAsync();
        }
    }
}
