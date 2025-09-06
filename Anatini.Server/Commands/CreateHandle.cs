using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateHandle(Guid handleId, string handleValue, Guid userId) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var handle = new Handle
            {
                Id = handleId,
                Value = handleValue,
                UserId = userId,
            };

            context.Add(handle);

            return await context.SaveChangesAsync();
        }
    }
}
