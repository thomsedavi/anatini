using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserChannelEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserChannelEdge> userChannelEdgeBuilder)
        {
            userChannelEdgeBuilder.ToTable("user_channel_edges");

            userChannelEdgeBuilder.HasKey(userChannelEdge => new { userChannelEdge.SourceUserId, userChannelEdge.TargetChannelId, userChannelEdge.Label });

            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.SourceUserId).Has(order: 0);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.TargetChannelId).Has(order: 1);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.Label).Has(order: 2);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.CreatedAtUtc).Has(order: 3);

            userChannelEdgeBuilder.HasOneWithMany(userChannelEdge => userChannelEdge.SourceUser, user => user.ChannelEdges, userChannelEdge => userChannelEdge.SourceUserId, DeleteBehavior.Restrict);
            userChannelEdgeBuilder.HasOneWithMany(userChannelEdge => userChannelEdge.TargetChannel, channel => channel.UserEdges, userChannelEdge => userChannelEdge.TargetChannelId, DeleteBehavior.Restrict);

            userChannelEdgeBuilder.HasIndex(userChannelEdge => new { userChannelEdge.TargetChannelId, userChannelEdge.Label, userChannelEdge.SourceUserId });
        }
    }
}
