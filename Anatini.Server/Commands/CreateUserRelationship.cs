using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateUserRelationships(Guid userId, Guid toUser, params string[] types) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            foreach (var type in types)
            {
                var userEvent = new UserRelationship
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ToUserId = toUser,
                    Type = type
                };

                context.Add(userEvent);
            }

            return await context.SaveChangesAsync();
        }
    }
}
