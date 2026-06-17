using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserSpaceEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserSpaceEdge> userSpaceEdgeBuilder)
        {
            userSpaceEdgeBuilder.ToTable("user_space_edges");

            userSpaceEdgeBuilder.HasKey(userSpaceEdge => new { userSpaceEdge.SourceUserId, userSpaceEdge.TargetSpaceId, userSpaceEdge.Label });

            userSpaceEdgeBuilder.Property(userSpaceEdge => userSpaceEdge.SourceUserId).Has(order: 0);
            userSpaceEdgeBuilder.Property(userSpaceEdge => userSpaceEdge.TargetSpaceId).Has(order: 1);
            userSpaceEdgeBuilder.Property(userSpaceEdge => userSpaceEdge.Label).Has(order: 2);
            userSpaceEdgeBuilder.Property(userSpaceEdge => userSpaceEdge.CreatedAtUtc).Has(order: 3);

            userSpaceEdgeBuilder.HasOneWithMany(userSpaceEdge => userSpaceEdge.SourceUser, user => user.SpaceEdges, userSpaceEdge => userSpaceEdge.SourceUserId, DeleteBehavior.Restrict);
            userSpaceEdgeBuilder.HasOneWithMany(userSpaceEdge => userSpaceEdge.TargetSpace, space => space.UserEdges, userSpaceEdge => userSpaceEdge.TargetSpaceId, DeleteBehavior.Restrict);

            userSpaceEdgeBuilder.HasIndex(userSpaceEdge => new { userSpaceEdge.TargetSpaceId, userSpaceEdge.Label, userSpaceEdge.SourceUserId });
        }
    }
}
