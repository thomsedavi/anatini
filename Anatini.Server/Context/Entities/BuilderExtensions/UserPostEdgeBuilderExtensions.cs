using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserPostEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserPostEdge> userPostEdgeBuilder)
        {
            userPostEdgeBuilder.ToTable("user_post_edges");

            userPostEdgeBuilder.HasKey(userPostEdge => new { userPostEdge.SourceUserId, userPostEdge.TargetPostId, userPostEdge.Label });

            userPostEdgeBuilder.Property(userPostEdge => userPostEdge.SourceUserId).Has(order: 0);
            userPostEdgeBuilder.Property(userPostEdge => userPostEdge.TargetPostId).Has(order: 1);
            userPostEdgeBuilder.Property(userPostEdge => userPostEdge.Label).Has(order: 2);
            userPostEdgeBuilder.Property(userPostEdge => userPostEdge.CreatedAtUtc).Has(order: 3);

            userPostEdgeBuilder.HasOneWithMany(userPostEdge => userPostEdge.SourceUser, user => user.PostEdges, userPostEdge => userPostEdge.SourceUserId, DeleteBehavior.Restrict);
            userPostEdgeBuilder.HasOneWithMany(userPostEdge => userPostEdge.TargetPost, post => post.UserEdges, userPostEdge => userPostEdge.TargetPostId, DeleteBehavior.Restrict);

            userPostEdgeBuilder.HasIndex(userPostEdge => new { userPostEdge.TargetPostId, userPostEdge.Label, userPostEdge.SourceUserId });
        }
    }
}
