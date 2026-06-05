using Anatini.Server.Dtos;

namespace Anatini.Server.Authentication.Responses
{
    public class IsAuthenticatedResponse
    {
        public bool IsAuthenticated { get; set; } = false;
        public bool? IsTrusted { get; set; }
        public IEnumerable<ChannelEditDto>? Channels { get; set; }
    }
}
