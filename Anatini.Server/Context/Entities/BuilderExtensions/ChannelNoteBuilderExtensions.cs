using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ChannelNoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ChannelNote> channelNoteBuilder)
        {
            channelNoteBuilder.ToTable("channel_notes");

            channelNoteBuilder.HasKey(channelNote => new { channelNote.ChannelId, channelNote.Handle });

            channelNoteBuilder.Property(channelNote => channelNote.ChannelId).Has(order: 0);
            channelNoteBuilder.Property(channelNote => channelNote.Handle)!.Has(maxLength: 256, order: 1);
            channelNoteBuilder.Property(channelNote => channelNote.NoteId).Has(order: 2);
            channelNoteBuilder.Property(channelNote => channelNote.CreatedAtUtc).Has(order: 3);

            channelNoteBuilder.HasOneWithMany(channelNote => channelNote.Channel, user => user.ChannelNotes, channelNote => channelNote.ChannelId, DeleteBehavior.Restrict);
            channelNoteBuilder.HasOneWithMany(channelNote => channelNote.Note, note => note.ChannelNotes, channelNote => channelNote.NoteId, DeleteBehavior.Restrict);

            channelNoteBuilder.HasIndex(channelNote => new { channelNote.ChannelId, channelNote.NoteId });
        }
    }
}
