using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Note> noteBuilder)
        {
            noteBuilder.ToContainer("Notes");
            noteBuilder.HasKey(note => new { note.ChannelId, note.Id });
            noteBuilder.HasPartitionKey(note => new { note.ChannelId, note.Id });
            noteBuilder.Property(note => note.ItemId).ToJsonProperty("id");
            noteBuilder.Property(note => note.ETag).ToJsonProperty("_etag");
            noteBuilder.Property(note => note.Timestamp).ToJsonProperty("_ts");
        }
    }
}
