using Anatini.Server.Context;
using Anatini.Server.Enums;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateUserToUserRelationships(Guid userId, Guid toUserId, params UserToUserRelationshipType[] types) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            foreach (var type in types)
            {
                var userToUserRelationship = new UserToUserRelationship
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ToUserId = toUserId,
                    Type = Enum.GetName(type)!
                };

                context.Add(userToUserRelationship);
            }

            return await context.SaveChangesAsync();
        }
    }
}
