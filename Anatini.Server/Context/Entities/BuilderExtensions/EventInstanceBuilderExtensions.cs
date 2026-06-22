using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventInstanceBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventInstance> eventInstanceBuilder)
        {
            eventInstanceBuilder.ToTable("event_instances", tableBuilder => tableBuilder.HasCheckConstraint("ck_event_instances_user_id_xor_space_id", $"({eventInstanceBuilder.GetColumnName(nameof(EventInstance.UserId))} IS NULL AND {eventInstanceBuilder.GetColumnName(nameof(EventInstance.SpaceId))} IS NOT NULL) OR ({eventInstanceBuilder.GetColumnName(nameof(EventInstance.SpaceId))} IS NULL AND {eventInstanceBuilder.GetColumnName(nameof(EventInstance.UserId))} IS NOT NULL)"));

            eventInstanceBuilder.HasKey(eventInstance => eventInstance.Id);

            eventInstanceBuilder.HasOneWithMany(content => content.User, user => user.EventInstances, content => content.UserId, DeleteBehavior.Restrict, required: false);
            eventInstanceBuilder.HasOneWithMany(content => content.Space, space => space.EventInstances, content => content.SpaceId, DeleteBehavior.Restrict, required: false);
        }
    }
}
