using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserNoteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserNote> userNoteBuilder)
        {
            userNoteBuilder.ToTable("user_notes");

            userNoteBuilder.HasKey(userNote => new { userNote.UserId, userNote.Handle });

            userNoteBuilder.Property(userNote => userNote.UserId).Has(order: 0);
            userNoteBuilder.Property(userNote => userNote.Handle)!.Has(maxLength: 256, order: 1);
            userNoteBuilder.Property(userNote => userNote.NoteId).Has(order: 2);
            userNoteBuilder.Property(userNote => userNote.CreatedAtUtc).Has(order: 3);

            userNoteBuilder.HasOneWithMany(userNote => userNote.User, user => user.UserNotes, userNote => userNote.UserId, DeleteBehavior.Restrict);
            userNoteBuilder.HasOneWithMany(userNote => userNote.Note, note => note.UserNotes, userNote => userNote.NoteId, DeleteBehavior.Restrict);

            userNoteBuilder.HasIndex(userNote => new { userNote.UserId, userNote.NoteId });
        }
    }
}
