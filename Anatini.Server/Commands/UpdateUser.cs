using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class UpdateUser(User user) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Update(user);

            return await context.SaveChangesAsync();
        }
    }
}
