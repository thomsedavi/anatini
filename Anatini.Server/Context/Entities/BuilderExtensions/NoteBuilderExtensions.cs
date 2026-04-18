using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Note> noteBuilder)
        {
            noteBuilder.ToTable("notes");

            noteBuilder.HasKey(note => note.Id);

            noteBuilder.Property(note => note.Id).Has(order: 0);
            noteBuilder.Property(note => note.ChannelId).Has(order: 1);
            noteBuilder.Property(note => note.Status).Has(order: 2);
            noteBuilder.Property(note => note.Handle)!.Has(maxLength: 256, order: 3);
            noteBuilder.Property(note => note.PublishedAtUtc).Has(order: 4);
            noteBuilder.Property(note => note.Visibility).Has(order: 5);
            noteBuilder.Property(note => note.Article)!.Has(order: 6);
            noteBuilder.Property(note => note.ConcurrencyStamp)!.Has(order: 7).IsConcurrencyToken();
            noteBuilder.Property(note => note.CreatedAtUtc).Has(order: 8);
            noteBuilder.Property(note => note.UpdatedAtUtc).Has(order: 9);

            noteBuilder.HasOneWithMany(note => note.Channel, channel => channel.Notes, note => note.ChannelId, DeleteBehavior.Cascade);

            noteBuilder.HasIndex(note => note.PublishedAtUtc);
            noteBuilder.HasIndex(note => new { note.ChannelId, note.PublishedAtUtc });
            noteBuilder.HasIndex(note => new { note.ChannelId, note.Handle }).IsUnique();
        }
    }
}
