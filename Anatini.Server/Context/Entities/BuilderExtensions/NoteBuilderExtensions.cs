using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class NoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Note> noteBuilder)
        {
            noteBuilder.ToTable("notes");

            noteBuilder.Property(note => note.Id).Has(order: 0);
        }
    }
}
