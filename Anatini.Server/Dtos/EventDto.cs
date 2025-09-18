using Anatini.Server.Context;

namespace Anatini.Server.Dtos
{
    internal class EventDto(Event @event)
    {
        public string Type { get; } = @event.Type;
        public DateTime DateUtc { get; } = @event.DateUtc;
    }
}
