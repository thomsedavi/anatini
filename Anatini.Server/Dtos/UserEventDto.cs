namespace Anatini.Server.Dtos
{
    public class UserEventDto
    {
        public required string EventType { get; set; }
        public required DateTime DateTimeUtc { get; set; }
    }
}
