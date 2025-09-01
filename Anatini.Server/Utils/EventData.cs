namespace Anatini.Server.Utils
{
    public class EventData
    {
        public DateTime DateTimeUtc { get; }
        public DateOnly DateOnlyNZNow { get; }

        private readonly Dictionary<string, string> Dictionary = [];

        public EventData(HttpContext httpContext)
        {
            Dictionary["IPAddress"] = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            Dictionary["UserAgent"] = httpContext.Request.Headers.UserAgent.ToString();

            DateTimeUtc = DateTime.UtcNow;

            var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            var dateTimeNZ = TimeZoneInfo.ConvertTimeFromUtc(DateTimeUtc, timeZoneInfoNZ);
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
