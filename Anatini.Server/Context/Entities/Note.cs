namespace Anatini.Server.Context.Entities
{
    public class Note : BaseEntity
    {
        public required string ChannelId { get; set; }
        public bool? Protected { get; set; }
    }

    public class AttributeNote : NoteEntity
    {
        public required string Value { get; set; }
    }

    public abstract class NoteEntity : Entity
    {
        public required string NoteId { get; set; }
        public required string NoteChannelId { get; set; }
    }
}
