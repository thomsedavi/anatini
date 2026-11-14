namespace Anatini.Server.Dtos
{
    public class EventOccurrenceDto : ContentDto
    {
        public required DateTime StartsAtNz { get; set; }
        public required DateTime EndsAtNz { get; set; }
        public required string Title { get; set; }
        public required EventHeaderDto EventHeader { get; set; }
    }
}
