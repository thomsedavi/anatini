namespace Anatini.Server.Controllers
{
    internal class EventData
    {
        private readonly Dictionary<string, string> Dictionary = [];

        public EventData(HttpContext httpContext)
        {
            Dictionary["IPAddress"] = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
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
