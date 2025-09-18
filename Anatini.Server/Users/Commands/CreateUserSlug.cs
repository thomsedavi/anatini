using Anatini.Server.Context;
using Anatini.Server.Interfaces;

namespace Anatini.Server.Users.Commands
{
    public class CreateUserSlug(NewUserSlug newSlug, User user) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userSlug = new UserSlug
            {
                Id = newSlug.Id,
                Slug = newSlug.Slug,
                UserId = user.Id,
                UserName = user.Name
            };

            context.Add(userSlug);

            return await context.SaveChangesAsync();
        }
    }
}
