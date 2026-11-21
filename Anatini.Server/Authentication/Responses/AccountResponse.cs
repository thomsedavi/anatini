using Anatini.Server.Dtos;

namespace Anatini.Server.Authentication.Responses
{
    public class AccountResponse
    {
        public bool? IsAuthenticated { get; set; }
        public bool? IsTrusted { get; set; }
        public Guid? UserId { get; set; }
        public string? UserHandle { get; set; }
        public IEnumerable<SpaceEditDto>? Spaces { get; set; }
    }
}
