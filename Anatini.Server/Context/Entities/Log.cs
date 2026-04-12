using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities
{
    public class Log
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ChannelId { get; set; }
        public required EventType EventType { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime DateTimeUtc { get; set; }
        public MetaData? MetaData { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Channel? Channel { get; set; }
    }

    public class MetaData
    {
    }
}
