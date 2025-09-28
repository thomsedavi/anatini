using Anatini.Server.Context;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserSlugExtensions
    {
        public static UserAlias Create(this NewUserAlias newUserSlug, User user)
        {
            return new UserAlias
            {
                Slug = newUserSlug.Slug,
                UserId = user.Id,
                UserName = user.Name
            };
        }
    }
}
