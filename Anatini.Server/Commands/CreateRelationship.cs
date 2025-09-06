using Anatini.Server.Enums;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateRelationships(Guid userId, Guid toUserId, params RelationshipType[] types) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            foreach (var type in types)
            {
                var relationship = new Relationship
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ToUserId = toUserId,
                    Type = Enum.GetName(type)!
                };

                context.Add(relationship);
            }

            return await context.SaveChangesAsync();
        }
    }
}
