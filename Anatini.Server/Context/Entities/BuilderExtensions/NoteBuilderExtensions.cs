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

            noteBuilder.Property(note => note.Id).HasColumnOrder(0);
            noteBuilder.Property(note => note.ChannelId).HasColumnOrder(1);
            noteBuilder.Property(note => note.Status).HasColumnOrder(2);
            noteBuilder.Property(note => note.Visibility).HasColumnOrder(3);
            noteBuilder.Property(note => note.Article).HasMaxLength(512).HasColumnOrder(4);
            noteBuilder.Property(note => note.ConcurrencyStamp).IsConcurrencyToken().HasColumnOrder(9);
            noteBuilder.Property(note => note.CreatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(10);
            noteBuilder.Property(note => note.UpdatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(11);

            noteBuilder.HasOneWithMany(note => note.Channel, channel => channel.Notes, note => note.ChannelId, DeleteBehavior.Cascade);
        }
    }
}
