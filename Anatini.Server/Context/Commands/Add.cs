using Anatini.Server.Interfaces;

namespace Anatini.Server.Context.Commands
{
    public class Add(Entity entity) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Add(entity);

            return await context.SaveChangesAsync();
        }
    }
}
