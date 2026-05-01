using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<NoteImage> noteImageBuilder)
        {
            noteImageBuilder.ToTable("note_images");

            noteImageBuilder.HasKey(noteImage => new { noteImage.NoteId, noteImage.Handle });

            noteImageBuilder.Property(noteImage => noteImage.NoteId).Has(order: 0);
            noteImageBuilder.Property(noteImage => noteImage.Handle)!.Has(maxLength: 256, order: 1);
            noteImageBuilder.Property(noteImage => noteImage.BlobName)!.Has(maxLength: 64, order: 2);
            noteImageBuilder.Property(noteImage => noteImage.BlobContainerName)!.Has(maxLength: 16, order: 3);
            noteImageBuilder.Property(noteImage => noteImage.AltText).Has(maxLength: 512, order: 4);
            noteImageBuilder.Property(noteImage => noteImage.CreatedAtUtc).Has(order: 5);
            noteImageBuilder.Property(noteImage => noteImage.UpdatedAtUtc).Has(order: 6);

            noteImageBuilder.HasOneWithMany(noteImage => noteImage.Note, note => note.Images, noteImage => noteImage.NoteId, DeleteBehavior.Restrict);
        }
    }
}
