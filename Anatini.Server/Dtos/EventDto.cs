namespace Anatini.Server.Dtos
{
    public class EventDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required DateTime StartsAtNz { get; set; }
        public required DateTime EndsAtNz { get; set; }
        public required string Name { get; set; }
        public string? Article { get; set; }
        public string? Url { get; set; }
    }
}
