using Anatini.Server.Interfaces;

namespace Anatini.Server.Context.Commands
{
    public class Update(Entity entity) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Update(entity);

            return await context.SaveChangesAsync();
        }
    }
}
