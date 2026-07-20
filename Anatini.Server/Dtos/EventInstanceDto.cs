namespace Anatini.Server.Dtos
{
    public class EventInstanceDto
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public required DateTime StartsAtNz { get; set; }
        public required DateTime EndsAtNz { get; set; }
    }
}
