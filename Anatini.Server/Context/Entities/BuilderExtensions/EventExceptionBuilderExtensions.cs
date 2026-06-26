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

            eventExceptionBuilder.HasOneWithMany(eventException => eventException.Series, eventSeries => eventSeries.Exceptions, eventException => eventException.EventSeriesId, DeleteBehavior.Cascade);
        }
    }
}
