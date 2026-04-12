namespace Anatini.Server.Authentication.Responses
{
    public class IsAuthenticatedResponse
    {
        public required bool IsAuthenticated { get; set; }
        public bool? IsTrusted { get; set; }
    }
}
