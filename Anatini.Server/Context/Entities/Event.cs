using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Event
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required DateTime DateTimeNZ { get; set; }
        public required Visibility Visibility { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual Channel Channel { get; set; } = null!;
    }
}
