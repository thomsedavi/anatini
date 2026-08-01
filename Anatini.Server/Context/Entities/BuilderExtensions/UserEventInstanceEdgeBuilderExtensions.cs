using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserEventInstanceEdgeBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserEventInstanceEdge> userEventInstanceEdgeBuilder)
        {
            userEventInstanceEdgeBuilder.ToTable("user_event_instance_edges");

            userEventInstanceEdgeBuilder.HasKey(userEventInstanceEdge => new { userEventInstanceEdge.SourceUserId, userEventInstanceEdge.TargetEventInstanceId, userEventInstanceEdge.Label });

            userEventInstanceEdgeBuilder.Property(userEventInstanceEdge => userEventInstanceEdge.SourceUserId).Has(order: 0);
            userEventInstanceEdgeBuilder.Property(userEventInstanceEdge => userEventInstanceEdge.TargetEventInstanceId).Has(order: 1);
            userEventInstanceEdgeBuilder.Property(userEventInstanceEdge => userEventInstanceEdge.Label).Has(order: 2);
            userEventInstanceEdgeBuilder.Property(userEventInstanceEdge => userEventInstanceEdge.CreatedAtUtc).Has(order: 3);

            userEventInstanceEdgeBuilder.HasOneWithMany(userEventInstanceEdge => userEventInstanceEdge.SourceUser, user => user.EventInstanceEdges, userEventInstanceEdge => userEventInstanceEdge.SourceUserId, DeleteBehavior.Restrict);
            userEventInstanceEdgeBuilder.HasOneWithMany(userEventInstanceEdge => userEventInstanceEdge.TargetEventInstance, eventInstance => eventInstance.UserEdges, userEventInstanceEdge => userEventInstanceEdge.TargetEventInstanceId, DeleteBehavior.Restrict);

            userEventInstanceEdgeBuilder.HasIndex(userEventInstanceEdge => new { userEventInstanceEdge.TargetEventInstanceId, userEventInstanceEdge.Label, userEventInstanceEdge.SourceUserId });
        }
    }
}
