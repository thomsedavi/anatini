using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventSeriesBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventSeries> eventSeriesBuilder)
        {
            eventSeriesBuilder.ToTable("event_series");

            eventSeriesBuilder.Property(eventSeries => eventSeries.Id).Has(order: 0);
            eventSeriesBuilder.Property(eventSeries => eventSeries.StartsAtNz).Has(order: 1);
            eventSeriesBuilder.Property(eventSeries => eventSeries.EndsAtNz).Has(order: 2);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Duration).Has(order: 3);
            eventSeriesBuilder.Property(eventSeries => eventSeries.RecurrenceRule).Has(order: 4);
            eventSeriesBuilder.Property(eventSeries => eventSeries.ExpiresAtNz).Has(order: 5);
        }
    }
}
