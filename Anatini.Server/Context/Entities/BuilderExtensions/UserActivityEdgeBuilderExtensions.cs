using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserActivityEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserActivityEdge> userActivityEdgeBuilder)
        {
            userActivityEdgeBuilder.ToTable("user_activity_edges");

            userActivityEdgeBuilder.HasKey(userActivityEdge => new { userActivityEdge.SourceUserId, userActivityEdge.TargetActivityId, userActivityEdge.Label });

            userActivityEdgeBuilder.Property(userActivityEdge => userActivityEdge.SourceUserId).Has(order: 0);
            userActivityEdgeBuilder.Property(userActivityEdge => userActivityEdge.TargetActivityId).Has(order: 1);
            userActivityEdgeBuilder.Property(userActivityEdge => userActivityEdge.Label).Has(order: 2);
            userActivityEdgeBuilder.Property(userActivityEdge => userActivityEdge.CreatedAtUtc).Has(order: 3);

            userActivityEdgeBuilder.HasOneWithMany(userActivityEdge => userActivityEdge.SourceUser, user => user.ActivityEdges, userActivityEdge => userActivityEdge.SourceUserId, DeleteBehavior.Restrict);
            userActivityEdgeBuilder.HasOneWithMany(userActivityEdge => userActivityEdge.TargetActivity, activity => activity.UserEdges, userActivityEdge => userActivityEdge.TargetActivityId, DeleteBehavior.Restrict);

            userActivityEdgeBuilder.HasIndex(userActivityEdge => new { userActivityEdge.TargetActivityId, userActivityEdge.Label, userActivityEdge.SourceUserId });
        }
    }
}
