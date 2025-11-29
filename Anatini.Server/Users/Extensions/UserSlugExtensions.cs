using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Users.Extensions
{
    public static class UserSlugExtensions
    {
        public static UserDto ToUserDto(this UserAlias userAlias)
        {
            return new UserDto
            {
                Id = userAlias.UserId,
                Name = userAlias.UserName
            };
        }
    }
}
