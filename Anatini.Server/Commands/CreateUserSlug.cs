using Anatini.Server.Interfaces;

namespace Anatini.Server.Commands
{
    public class CreateUserSlug(Guid id, string slug, Guid userId, string userName) : ICommand<int>
    {
        public async Task<int> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            var userSlug = new UserSlug
            {
                Id = id,
                Slug = slug,
                UserId = userId,
                UserName = userName
            };

            context.Add(userSlug);

            return await context.SaveChangesAsync();
        }
    }
}
