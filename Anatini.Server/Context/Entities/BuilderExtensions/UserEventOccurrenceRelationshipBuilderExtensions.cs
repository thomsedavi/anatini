using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserEventOccurrenceRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserEventOccurrenceRelationship> userEventOccurrenceRelationshipBuilder)
        {
            userEventOccurrenceRelationshipBuilder.ToTable("user_event_occurrence_relationships");

            userEventOccurrenceRelationshipBuilder.HasKey(userEventOccurrenceRelationship => new { userEventOccurrenceRelationship.SourceUserId, userEventOccurrenceRelationship.TargetEventOccurrenceId, userEventOccurrenceRelationship.Label });

            userEventOccurrenceRelationshipBuilder.Property(userEventOccurrenceRelationship => userEventOccurrenceRelationship.SourceUserId).Has(order: 0);
            userEventOccurrenceRelationshipBuilder.Property(userEventOccurrenceRelationship => userEventOccurrenceRelationship.TargetEventOccurrenceId).Has(order: 1);
            userEventOccurrenceRelationshipBuilder.Property(userEventOccurrenceRelationship => userEventOccurrenceRelationship.Label).Has(order: 2);
            userEventOccurrenceRelationshipBuilder.Property(userEventOccurrenceRelationship => userEventOccurrenceRelationship.CreatedAtUtc).Has(order: 3);

            userEventOccurrenceRelationshipBuilder.HasOneWithMany(userEventOccurrenceRelationship => userEventOccurrenceRelationship.SourceUser, user => user.EventOccurrenceRelationships, userEventOccurrenceRelationship => userEventOccurrenceRelationship.SourceUserId, DeleteBehavior.Restrict);
            userEventOccurrenceRelationshipBuilder.HasOneWithMany(userEventOccurrenceRelationship => userEventOccurrenceRelationship.TargetEventOccurrence, eventOccurrence => eventOccurrence.UserRelationships, userEventOccurrenceRelationship => userEventOccurrenceRelationship.TargetEventOccurrenceId, DeleteBehavior.Restrict);

            userEventOccurrenceRelationshipBuilder.HasIndex(userEventOccurrenceRelationship => new { userEventOccurrenceRelationship.TargetEventOccurrenceId, userEventOccurrenceRelationship.Label, userEventOccurrenceRelationship.SourceUserId });
        }
    }
}
