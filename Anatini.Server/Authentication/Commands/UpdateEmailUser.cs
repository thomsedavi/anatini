using Anatini.Server.Interfaces;

namespace Anatini.Server.Authentication.Commands
{
    internal class UpdateEmailUser(EmailUser emailUser) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Update(emailUser);

            return await context.SaveChangesAsync();
        }
    }
}
