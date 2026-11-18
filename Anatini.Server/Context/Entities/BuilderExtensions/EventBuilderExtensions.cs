using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Event> eventBuilder)
        {
            eventBuilder.ToTable("events");

            eventBuilder.Property(@event => @event.Id).Has(order: 0);
            eventBuilder.Property(@event => @event.StartsAtUtc).Has(order: 1);
            eventBuilder.Property(@event => @event.EndsAtUtc).Has(order: 2);
            eventBuilder.Property(@event => @event.Duration).Has(order: 3);
            eventBuilder.Property(@event => @event.RecurrenceRule).Has(order: 4);
            eventBuilder.Property(@event => @event.ExpiresAtUtc).Has(order: 5);
        }
    }
}
