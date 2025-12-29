using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserAliasContextExtensions
    {
        public static async Task<UserAlias> AddUserAliasAsync(this AnatiniContext context, Guid userId, string slug, string userName, bool? @protected)
        {
            var userAlias = new UserAlias
            {
                ItemId = ItemId.Get(slug),
                Slug = slug,
                UserId = userId,
                UserName = userName,
                Protected = @protected
            };

            await context.AddAsync(userAlias);

            return userAlias;
        }
    }
}
