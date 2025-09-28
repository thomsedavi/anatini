namespace Anatini.Server.Dtos
{
    public class UserEventDto
    {
        public required string Type { get; set; }
        public required DateTime DateTimeUtc { get; set; }
    }
}
