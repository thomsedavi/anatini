using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class AttributeNoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<AttributeNote> attributeNoteBuilder)
        {
            attributeNoteBuilder.ToContainer("AttributeNotes");
            attributeNoteBuilder.HasKey(attributeNote => new { attributeNote.Value, attributeNote.NoteChannelId, attributeNote.NoteId });
            attributeNoteBuilder.HasPartitionKey(attributeNote => new { attributeNote.Value, attributeNote.NoteChannelId, attributeNote.NoteId });
            attributeNoteBuilder.Property(attributeNote => attributeNote.ItemId).ToJsonProperty("id");
            attributeNoteBuilder.Property(attributeNote => attributeNote.ETag).ToJsonProperty("_etag");
            attributeNoteBuilder.Property(attributeNote => attributeNote.Timestamp).ToJsonProperty("_ts");
        }
    }
}
