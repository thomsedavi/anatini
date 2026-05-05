using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Note> noteBuilder)
        {
            noteBuilder.ToTable("notes", tableBuilder => tableBuilder.HasCheckConstraint("ck_notes_user_id_xor_channel_id", $"({noteBuilder.GetColumnName(nameof(Note.UserId))} IS NULL AND {noteBuilder.GetColumnName(nameof(Note.ChannelId))} IS NOT NULL) OR ({noteBuilder.GetColumnName(nameof(Note.ChannelId))} IS NULL AND {noteBuilder.GetColumnName(nameof(Note.UserId))} IS NOT NULL)"));

            noteBuilder.HasKey(note => note.Id);

            noteBuilder.Property(note => note.Id).Has(order: 0);
            noteBuilder.Property(note => note.UserId).Has(order: 1);
            noteBuilder.Property(note => note.ChannelId).Has(order: 2);
            noteBuilder.Property(note => note.Handle)!.Has(maxLength: 256, order: 3);
            noteBuilder.Property(note => note.Status).Has(order: 4);
            noteBuilder.Property(note => note.PublishedAtUtc).Has(order: 5);
            noteBuilder.Property(note => note.Visibility).Has(order: 6);
            noteBuilder.Property(note => note.Article)!.Has(order: 7);
            noteBuilder.Property(note => note.ConcurrencyStamp)!.Has(order: 8).IsConcurrencyToken();
            noteBuilder.Property(note => note.CreatedAtUtc).Has(order: 9);
            noteBuilder.Property(note => note.UpdatedAtUtc).Has(order: 10);

            noteBuilder.HasOneWithMany(note => note.User, user => user.Notes, note => note.UserId, DeleteBehavior.Restrict, required: false);
            noteBuilder.HasOneWithMany(note => note.Channel, channel => channel.Notes, note => note.ChannelId, DeleteBehavior.Restrict, required: false);

            noteBuilder.HasIndex(note => new { note.UserId, note.Handle }).IsUnique().HasFilter($"{noteBuilder.GetColumnName(nameof(Note.UserId))} IS NOT NULL");
            noteBuilder.HasIndex(note => new { note.ChannelId, note.Handle }).IsUnique().HasFilter($"{noteBuilder.GetColumnName(nameof(Note.ChannelId))} IS NOT NULL");
            noteBuilder.HasIndex(note => note.PublishedAtUtc);
        }
    }
}
