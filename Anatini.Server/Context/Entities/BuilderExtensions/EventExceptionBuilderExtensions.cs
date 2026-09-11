using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventExceptionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventException> eventExceptionBuilder)
        {
            eventExceptionBuilder.ToTable("event_exceptions");

            eventExceptionBuilder.HasKey(eventException => new { eventException.EventSeriesId, eventException.TargetStartsAtNz });

            eventExceptionBuilder.Property(eventException => eventException.EventSeriesId).Has(order: 0);
            eventExceptionBuilder.Property(eventException => eventException.TargetStartsAtNz).Has(order: 1);
            eventExceptionBuilder.Property(eventException => eventException.IsCancelled).Has(order: 2);
            eventExceptionBuilder.Property(eventException => eventException.OverrideHeader).Has(maxLength: 255, order: 3);
            eventExceptionBuilder.Property(eventException => eventException.OverrideArticle).Has(order: 4);
            eventExceptionBuilder.Property(eventException => eventException.OverrideStartsAtNz).Has(order: 5);
            eventExceptionBuilder.Property(eventException => eventException.OverrideDuration).Has(order: 6);
            eventExceptionBuilder.Property(eventException => eventException.OverrideEndsAtNz).Has(order: 7);

            eventExceptionBuilder.HasOneWithMany(eventException => eventException.Series, eventSeries => eventSeries.Exceptions, eventException => eventException.EventSeriesId, DeleteBehavior.Cascade);
        }
    }
}
