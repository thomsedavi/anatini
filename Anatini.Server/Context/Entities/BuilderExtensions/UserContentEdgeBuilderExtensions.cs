using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserContentEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserContentEdge> userContentEdgeBuilder)
        {
            userContentEdgeBuilder.ToTable("user_content_edges");

            userContentEdgeBuilder.HasKey(userContentEdge => new { userContentEdge.SourceUserId, userContentEdge.TargetContentId, userContentEdge.Label });

            userContentEdgeBuilder.Property(userContentEdge => userContentEdge.SourceUserId).Has(order: 0);
            userContentEdgeBuilder.Property(userContentEdge => userContentEdge.TargetContentId).Has(order: 1);
            userContentEdgeBuilder.Property(userContentEdge => userContentEdge.Label).Has(order: 2);
            userContentEdgeBuilder.Property(userContentEdge => userContentEdge.CreatedAtUtc).Has(order: 3);

            userContentEdgeBuilder.HasOneWithMany(userContentEdge => userContentEdge.SourceUser, user => user.ContentEdges, userContentEdge => userContentEdge.SourceUserId, DeleteBehavior.Restrict);
            userContentEdgeBuilder.HasOneWithMany(userContentEdge => userContentEdge.TargetContent, content => content.UserEdges, userContentEdge => userContentEdge.TargetContentId, DeleteBehavior.Restrict);

            userContentEdgeBuilder.HasIndex(userContentEdge => new { userContentEdge.TargetContentId, userContentEdge.Label, userContentEdge.SourceUserId });
        }
    }
}
