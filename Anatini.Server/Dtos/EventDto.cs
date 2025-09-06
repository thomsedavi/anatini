namespace Anatini.Server.Dtos
{
    internal class EventDto(Event @event)
    {
        public string Type { get; } = @event.Type;
        public DateTime CreatedDateUtc { get; } = @event.CreatedDateUtc;
    }
}
