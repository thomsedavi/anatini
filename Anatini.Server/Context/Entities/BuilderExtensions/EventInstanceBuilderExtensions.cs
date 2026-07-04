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

            eventInstanceBuilder.Property(eventInstance => eventInstance.Id).Has(order: 0);
            eventInstanceBuilder.Property(eventInstance => eventInstance.EventSeriesId).Has(order: 1);
            eventInstanceBuilder.Property(eventInstance => eventInstance.UserId).Has(order: 2);
            eventInstanceBuilder.Property(eventInstance => eventInstance.SpaceId).Has(order: 3);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Handle)!.Has(maxLength: 255, order: 4);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Status).Has(order: 5);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Visibility).Has(order: 6);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Name)!.Has(maxLength: 255, order: 7);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Article).Has(order: 8);
            eventInstanceBuilder.Property(eventInstance => eventInstance.Url).Has(maxLength: 2047, order: 9);
            eventInstanceBuilder.Property(eventInstance => eventInstance.StartsAtNz).Has(order: 10);
            eventInstanceBuilder.Property(eventInstance => eventInstance.EndsAtNz).Has(order: 11);

            eventInstanceBuilder.HasOneWithMany(eventInstance => eventInstance.Series, eventSeries => eventSeries.Instances, eventInstance => eventInstance.EventSeriesId, DeleteBehavior.Cascade);
            eventInstanceBuilder.HasOneWithMany(eventInstance => eventInstance.User, user => user.EventInstances, eventInstance => eventInstance.UserId, DeleteBehavior.Restrict, required: false);
            eventInstanceBuilder.HasOneWithMany(eventInstance => eventInstance.Space, space => space.EventInstances, eventInstance => eventInstance.SpaceId, DeleteBehavior.Restrict, required: false);
        }
    }
}
