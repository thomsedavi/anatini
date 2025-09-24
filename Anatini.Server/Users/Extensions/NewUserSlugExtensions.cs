using Anatini.Server.Authentication;
using Anatini.Server.Context;

namespace Anatini.Server.Users.Extensions
{
    public static class NewUserSlugExtensions
    {
        public static UserSlug Create(this NewUserSlug newUserSlug, User user)
        {
            return new UserSlug
            {
                Id = newUserSlug.Id,
                Slug = newUserSlug.Slug,
                UserId = user.Id,
                UserName = user.Name
            };
        }
    }
}
