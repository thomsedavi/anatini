using Anatini.Server.Utils;

namespace Anatini.Server.Context.EntityExtensions
{
    public static class UserAliasContextExtensions
    {
        public static async Task<UserAlias> AddUserAliasAsync(this AnatiniContext context, Guid userId, string slug, string userName)
        {
            var userAlias = new UserAlias
            {
                ItemId = ItemId.Get(slug),
                Slug = slug,
                UserId = userId,
                UserName = userName
            };

            await context.Add(userAlias);

            return userAlias;
        }
    }
}
