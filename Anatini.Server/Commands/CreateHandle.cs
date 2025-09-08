using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateHandle(Guid handleId, string handleValue, Guid userId, string userName) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var handleUser = new HandleUser
            {
                UserId = userId,
                HandleId = handleId,
                UserName = userName
            };

            var handle = new Handle
            {
                Id = handleId,
                Value = handleValue,
                Users = [handleUser],
            };

            context.Add(handle);

            return await context.SaveChangesAsync();
        }
    }
}
