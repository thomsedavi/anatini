using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserChannelEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserChannelEdge> userChannelEdgeBuilder)
        {
            userChannelEdgeBuilder.ToTable("user_channel_edges");

            userChannelEdgeBuilder.HasKey(userChannelEdge => new { userChannelEdge.UserId, userChannelEdge.ChannelId, userChannelEdge.Label });

            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.UserId).Has(order: 0);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.ChannelId).Has(order: 1);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.Label).Has(order: 2);
            userChannelEdgeBuilder.Property(userChannelEdge => userChannelEdge.CreatedAtUtc).Has(order: 3);

            userChannelEdgeBuilder.HasOneWithMany(userChannelEdge => userChannelEdge.User, user => user.ChannelEdges, userChannelEdge => userChannelEdge.UserId, DeleteBehavior.Restrict);
            userChannelEdgeBuilder.HasOneWithMany(userChannelEdge => userChannelEdge.Channel, channel => channel.UserEdges, userChannelEdge => userChannelEdge.ChannelId, DeleteBehavior.Restrict);

            userChannelEdgeBuilder.HasIndex(userChannelEdge => new { userChannelEdge.ChannelId, userChannelEdge.UserId, userChannelEdge.Label });
        }
    }
}
