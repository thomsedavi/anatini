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
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.StartsAtUtc).Has(order: 1);
            eventOccurrenceBuilder.Property(eventOccurrence => eventOccurrence.EndsAtUtc).Has(order: 2);

            eventOccurrenceBuilder.HasIndex(eventOccurrence => new { eventOccurrence.StartsAtUtc, eventOccurrence.EndsAtUtc });

            eventOccurrenceBuilder.Navigation(eventOccurrence => eventOccurrence.Parent).AutoInclude();
        }
    }
}
