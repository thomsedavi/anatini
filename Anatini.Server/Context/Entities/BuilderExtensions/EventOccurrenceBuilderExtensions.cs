using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventOccurrenceBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventOccurrence> eventOccurrenceBuilder)
        {
            eventOccurrenceBuilder.ToTable("event_occurrences");

            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.Id).Has(order: 0);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.EventId).Has(order: 1);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.StartsAtNz).Has(order: 2);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.EndsAtNz).Has(order: 3);

            eventOccurrenceBuilder.HasOneWithMany(eventOccurrence => eventOccurrence.Event, @event => @event.Occurrences, eventOccurrence => eventOccurrence.EventId, DeleteBehavior.Cascade);

            eventOccurrenceBuilder.HasIndex(eventOccurrence => new { eventOccurrence.StartsAtNz, eventOccurrence.EndsAtNz });

            eventOccurrenceBuilder.Navigation(eventOccurrence => eventOccurrence.Event).AutoInclude();
        }
    }
}
