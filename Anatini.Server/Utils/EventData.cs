namespace Anatini.Server.Utils
{
    public class EventData
    {
        public DateTime DateTimeUtc { get; }
        public DateOnly DateOnlyNZNow { get; }

        private readonly Dictionary<string, string> Dictionary = [];

        public EventData(HttpContext httpContext)
        {
            Dictionary["ipAddress"] = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            Dictionary["userAgent"] = httpContext.Request.Headers.UserAgent.ToString();

            DateTimeUtc = DateTime.UtcNow;

            var dateTimeNZ = DateTimeUtc.ConvertUtcToNz();
            DateOnlyNZNow = DateOnly.FromDateTime(dateTimeNZ);
        }

        internal string Get(string key)
        {
            return Dictionary[key];
        }

        internal EventData Add(string key, string value)
        {
            Dictionary[key] = value;

            return this;
        }

        internal IDictionary<string, string> ToDictionary()
        {
            return Dictionary;
        }
    }
}
