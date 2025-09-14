using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    internal class UpdateInvite(Invite invite) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            context.Update(invite);

            return await context.SaveChangesAsync();
        }
    }
}
