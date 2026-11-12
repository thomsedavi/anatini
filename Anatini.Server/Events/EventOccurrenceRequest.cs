namespace Anatini.Server.Events
{
    public class EventOccurrencesQuery
    {
        public string? Bookmarked { get; set; }
        public string? Starred { get; set; }
        public string? Dismissed { get; set; }
        public string? Followed { get; set; }
        public string? Collected { get; set; }
    }
}
