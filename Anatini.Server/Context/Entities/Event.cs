namespace Anatini.Server.Context.Entities
{
    public class Event : BaseEntity
    {
        public required string ChannelId { get; set; }
        public bool? Protected { get; set; }
    }
}
