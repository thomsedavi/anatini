using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateHandle(Guid handleId, Guid userId, string handle) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var handleUser = new HandleUser
            {
                Id = handleId,
                UserId = userId,
                Handle = handle
            };

            context.Add(handleUser);

            return await context.SaveChangesAsync();
        }
    }
}
