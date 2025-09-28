using Anatini.Server.Context;
using Anatini.Server.Dtos;

namespace Anatini.Server.Users.Extensions
{
    public static class UserEventExtensions
    {
        public static UserEventDto ToUserEventDto(this UserEvent userEvent)
        {
            return new UserEventDto
            {
                Type = userEvent.Type,
                DateTimeUtc = userEvent.DateTimeUtc
            };
        }
    }
}
