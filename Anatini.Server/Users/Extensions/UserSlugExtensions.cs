using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Users.Extensions
{
    public static class UserSlugExtensions
    {
        public static UserDto ToUserDto(this UserSlug userSlug)
        {
            return new UserDto
            {
                Id = userSlug.UserId,
                Name = userSlug.UserName
            };
        }
    }
}
