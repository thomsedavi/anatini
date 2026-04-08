using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<NoteImage> noteImageBuilder)
        {
            noteImageBuilder.ToTable("note_images");

            noteImageBuilder.HasKey(noteImage => noteImage.Id);

            noteImageBuilder.Property(noteImage => noteImage.Id).HasColumnOrder(0);
            noteImageBuilder.Property(noteImage => noteImage.NoteId).HasColumnOrder(1);
            noteImageBuilder.Property(noteImage => noteImage.BlobName).HasMaxLength(64).HasColumnOrder(2);
            noteImageBuilder.Property(noteImage => noteImage.BlobContainerName).HasMaxLength(16).HasColumnOrder(3);
            noteImageBuilder.Property(noteImage => noteImage.AltText).HasMaxLength(256).HasColumnOrder(4);
            noteImageBuilder.Property(noteImage => noteImage.CreatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(5);
            noteImageBuilder.Property(noteImage => noteImage.UpdatedAtUtc).HasColumnType("timestamp with time zone").HasColumnOrder(6);

            noteImageBuilder.HasOne(noteImage => noteImage.Note).WithMany(user => user.Images).HasForeignKey(noteImage => noteImage.NoteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
