using Anatini.Server.Context;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserSlugExtensions
    {
        public static UserAlias Create(this NewUserAlias newUserSlug, User user)
        {
            return new UserAlias
            {
                Guid = newUserSlug.Guid,
                Slug = newUserSlug.Slug,
                UserGuid = user.Guid,
                UserName = user.Name
            };
        }
    }
}
