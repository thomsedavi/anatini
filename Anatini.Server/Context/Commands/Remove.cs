using Anatini.Server.Interfaces;

namespace Anatini.Server.Context.Commands
{
    public class Remove(Entity entity) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Remove(entity);

            return await context.SaveChangesAsync();
        }
    }
}
