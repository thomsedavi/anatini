using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserNoteEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserNoteEdge> userNoteEdgeBuilder)
        {
            userNoteEdgeBuilder.ToTable("user_note_edges");

            userNoteEdgeBuilder.HasKey(userNoteEdge => new { userNoteEdge.SourceUserId, userNoteEdge.TargetNoteId, userNoteEdge.Label });

            userNoteEdgeBuilder.Property(userNoteEdge => userNoteEdge.SourceUserId).Has(order: 0);
            userNoteEdgeBuilder.Property(userNoteEdge => userNoteEdge.TargetNoteId).Has(order: 1);
            userNoteEdgeBuilder.Property(userNoteEdge => userNoteEdge.Label).Has(order: 2);
            userNoteEdgeBuilder.Property(userNoteEdge => userNoteEdge.CreatedAtUtc).Has(order: 3);

            userNoteEdgeBuilder.HasOneWithMany(userNoteEdge => userNoteEdge.SourceUser, user => user.NoteEdges, userNoteEdge => userNoteEdge.SourceUserId, DeleteBehavior.Restrict);
            userNoteEdgeBuilder.HasOneWithMany(userNoteEdge => userNoteEdge.TargetNote, note => note.UserEdges, userNoteEdge => userNoteEdge.TargetNoteId, DeleteBehavior.Restrict);

            userNoteEdgeBuilder.HasIndex(userNoteEdge => new { userNoteEdge.TargetNoteId, userNoteEdge.Label, userNoteEdge.SourceUserId });
        }
    }
}
