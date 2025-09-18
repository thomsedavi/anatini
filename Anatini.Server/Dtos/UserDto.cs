using Anatini.Server.Context;

namespace Anatini.Server.Dtos
{
    internal class UserDto(User user)
    {
        public string Name { get; } = user.Name;
    }
}
