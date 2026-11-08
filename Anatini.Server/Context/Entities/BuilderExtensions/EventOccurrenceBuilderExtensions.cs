using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventOccurrenceBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventOccurrence> eventOccurrenceBuilder)
        {
            eventOccurrenceBuilder.ToTable("event_occurrences", tableBuilder => tableBuilder.HasCheckConstraint("ck_event_occurrences_user_id_xor_space_id", $"({eventOccurrenceBuilder.GetColumnName(nameof(Event.UserId))} IS NULL AND {eventOccurrenceBuilder.GetColumnName(nameof(Event.SpaceId))} IS NOT NULL) OR ({eventOccurrenceBuilder.GetColumnName(nameof(EventOccurrence.SpaceId))} IS NULL AND {eventOccurrenceBuilder.GetColumnName(nameof(EventOccurrence.UserId))} IS NOT NULL)"));

            eventOccurrenceBuilder.HasKey(eventOccurrence => eventOccurrence.Id);

            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Id).Has(order: 0);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.EventId).Has(order: 1);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.UserId).Has(order: 2);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.SpaceId).Has(order: 3);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Handle)!.Has(maxLength: 255, order: 4);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Status).Has(order: 5);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.PublishedAtNz).Has(order: 6);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Visibility).Has(order: 7);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Title)!.Has(maxLength: 255, order: 8);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Body)!.Has(order: 9);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.StartsAtNz).Has(order: 10);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.EndsAtNz).Has(order: 11);

            eventOccurrenceBuilder.HasOneWithMany(eventOccurrence => eventOccurrence.Event, @event => @event.Occurrences, eventOccurrence => eventOccurrence.EventId, DeleteBehavior.Cascade);
            eventOccurrenceBuilder.HasOneWithMany(eventOccurrence => eventOccurrence.User, user => user.EventOccurrences, eventOccurrence => eventOccurrence.UserId, DeleteBehavior.Restrict, required: false);
            eventOccurrenceBuilder.HasOneWithMany(eventOccurrence => eventOccurrence.Space, space => space.EventOccurrences, eventOccurrence => eventOccurrence.SpaceId, DeleteBehavior.Restrict, required: false);

            eventOccurrenceBuilder.HasIndex(eventOccurrence => new { eventOccurrence.StartsAtNz, eventOccurrence.EndsAtNz });

            eventOccurrenceBuilder.HasGeneratedTsVectorColumn(eventOccurrence => eventOccurrence.SearchVector, "english", eventOccurrence => new { eventOccurrence.Title, eventOccurrence.Body }).HasIndex(eventOccurrence => eventOccurrence.SearchVector).HasMethod("GIN");

            eventOccurrenceBuilder.Navigation(eventOccurrence => eventOccurrence.Event).AutoInclude();
        }
    }
}
