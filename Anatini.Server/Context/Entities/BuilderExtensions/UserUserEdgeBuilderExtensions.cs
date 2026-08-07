using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserUserEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserUserEdge> userUserEdgeBuilder)
        {
            userUserEdgeBuilder.ToTable("user_user_edges", tableBuilder => tableBuilder.HasCheckConstraint("ck_user_user_edges_source_user_id_not_target_user_id", $"{userUserEdgeBuilder.GetColumnName(nameof(ApplicationUserUserEdge.SourceUserId))} <> {userUserEdgeBuilder.GetColumnName(nameof(ApplicationUserUserEdge.TargetUserId))}"));

            userUserEdgeBuilder.HasKey(userUserEdge => new { userUserEdge.SourceUserId, userUserEdge.TargetUserId, userUserEdge.Label });

            userUserEdgeBuilder.Property(userUserEdge => userUserEdge.SourceUserId).Has(order: 0);
            userUserEdgeBuilder.Property(userUserEdge => userUserEdge.TargetUserId).Has(order: 1);
            userUserEdgeBuilder.Property(userUserEdge => userUserEdge.Label).Has(order: 2);
            userUserEdgeBuilder.Property(userUserEdge => userUserEdge.CreatedAtUtc).Has(order: 3);

            userUserEdgeBuilder.HasOneWithMany(userUserEdge => userUserEdge.SourceUser, user => user.GivenUserEdges, userUserEdge => userUserEdge.SourceUserId, DeleteBehavior.Restrict);
            userUserEdgeBuilder.HasOneWithMany(userUserEdge => userUserEdge.TargetUser, user => user.ReceivedUserEdges, userUserEdge => userUserEdge.TargetUserId, DeleteBehavior.Restrict);

            userUserEdgeBuilder.HasIndex(userUserEdge => new { userUserEdge.TargetUserId, userUserEdge.Label, userUserEdge.SourceUserId });
        }
    }
}
