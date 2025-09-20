using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Users.Extensions
{
    public static class UserSlugExtensions
    {
        public static UserDto ToUserDto(this UserSlug userSlug)
        {
            var userDto = new UserDto
            {
                Id = userSlug.UserId,
                Name = userSlug.UserName
            };

            return userDto;
        }
    }
}
