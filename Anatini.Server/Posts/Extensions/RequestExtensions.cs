namespace Anatini.Server.Posts.Extensions
{
    public static class RequestExtensions
    {
        public static string? ETagHeader(this HttpRequest request)
        {
            return request.Headers.IfMatch.FirstOrDefault();
        }
    }
}
