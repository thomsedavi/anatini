namespace Anatini.Server.Dtos
{
    public class EventDto
    {
        public required string Type { get; set; }
        public required DateTime DateUtc { get; set; }
    }
}
