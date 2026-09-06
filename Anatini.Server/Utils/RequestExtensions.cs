namespace Anatini.Server.Utils
{
    public static class RequestExtensions
    {
        public static string? ETagHeader(this HttpRequest request)
        {
            return request.Headers.IfMatch.FirstOrDefault();
        }
    }
}
