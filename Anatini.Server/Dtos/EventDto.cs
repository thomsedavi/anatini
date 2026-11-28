namespace Anatini.Server.Dtos
{
    public class EventDto : ContentDto
    {
        public required DateTime StartsAtNz { get; set; }
        public DateTime? EndsAtNz { get; set; }
        public required string Title { get; set; }
    }

    public class EventHeaderDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required bool HasRecurrenceRule { get; set; }
    }
}
