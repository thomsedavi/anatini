using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    public class DeleteEmailUser(EmailUser emailUser) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Remove(emailUser);

            return await context.SaveChangesAsync();
        }
    }
}
