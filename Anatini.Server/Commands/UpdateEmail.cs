using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    internal class UpdateEmail(Email email) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Update(email);

            return await context.SaveChangesAsync();
        }
    }
}
