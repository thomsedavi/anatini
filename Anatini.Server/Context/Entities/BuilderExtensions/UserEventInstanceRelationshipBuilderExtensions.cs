using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserEventInstanceRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserEventInstanceRelationship> userEventInstanceRelationshipBuilder)
        {
            userEventInstanceRelationshipBuilder.ToTable("user_event_instance_relationships");

            userEventInstanceRelationshipBuilder.HasKey(userEventInstanceRelationship => new { userEventInstanceRelationship.SourceUserId, userEventInstanceRelationship.TargetEventInstanceId, userEventInstanceRelationship.Label });

            userEventInstanceRelationshipBuilder.Property(userEventInstanceRelationship => userEventInstanceRelationship.SourceUserId).Has(order: 0);
            userEventInstanceRelationshipBuilder.Property(userEventInstanceRelationship => userEventInstanceRelationship.TargetEventInstanceId).Has(order: 1);
            userEventInstanceRelationshipBuilder.Property(userEventInstanceRelationship => userEventInstanceRelationship.Label).Has(order: 2);
            userEventInstanceRelationshipBuilder.Property(userEventInstanceRelationship => userEventInstanceRelationship.CreatedAtUtc).Has(order: 3);

            userEventInstanceRelationshipBuilder.HasOneWithMany(userEventInstanceRelationship => userEventInstanceRelationship.SourceUser, user => user.EventInstanceRelationships, userEventInstanceRelationship => userEventInstanceRelationship.SourceUserId, DeleteBehavior.Restrict);
            userEventInstanceRelationshipBuilder.HasOneWithMany(userEventInstanceRelationship => userEventInstanceRelationship.TargetEventInstance, eventInstance => eventInstance.UserRelationships, userEventInstanceRelationship => userEventInstanceRelationship.TargetEventInstanceId, DeleteBehavior.Restrict);

            userEventInstanceRelationshipBuilder.HasIndex(userEventInstanceRelationship => new { userEventInstanceRelationship.TargetEventInstanceId, userEventInstanceRelationship.Label, userEventInstanceRelationship.SourceUserId });
        }
    }
}
