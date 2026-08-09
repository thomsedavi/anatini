using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserWorkEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserWorkEdge> userWorkEdgeBuilder)
        {
            userWorkEdgeBuilder.ToTable("user_work_edges");

            userWorkEdgeBuilder.HasKey(userWorkEdge => new { userWorkEdge.SourceUserId, userWorkEdge.TargetWorkId, userWorkEdge.Label });

            userWorkEdgeBuilder.Property(userWorkEdge => userWorkEdge.SourceUserId).Has(order: 0);
            userWorkEdgeBuilder.Property(userWorkEdge => userWorkEdge.TargetWorkId).Has(order: 1);
            userWorkEdgeBuilder.Property(userWorkEdge => userWorkEdge.Label).Has(order: 2);
            userWorkEdgeBuilder.Property(userWorkEdge => userWorkEdge.CreatedAtUtc).Has(order: 3);

            userWorkEdgeBuilder.HasOneWithMany(userWorkEdge => userWorkEdge.SourceUser, user => user.WorkEdges, userWorkEdge => userWorkEdge.SourceUserId, DeleteBehavior.Restrict);
            userWorkEdgeBuilder.HasOneWithMany(userWorkEdge => userWorkEdge.TargetWork, activity => activity.UserEdges, userWorkEdge => userWorkEdge.TargetWorkId, DeleteBehavior.Restrict);

            userWorkEdgeBuilder.HasIndex(userWorkEdge => new { userWorkEdge.TargetWorkId, userWorkEdge.Label, userWorkEdge.SourceUserId });
        }
    }
}
