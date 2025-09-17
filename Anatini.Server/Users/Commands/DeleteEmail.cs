using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class DeleteEmail(Email email) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Remove(email);

            return await context.SaveChangesAsync();
        }
    }
}
