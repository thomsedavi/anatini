using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class EventSeriesBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<EventSeries> eventSeriesBuilder)
        {
            eventSeriesBuilder.ToTable("event_series", tableBuilder => tableBuilder.HasCheckConstraint("ck_event_series_user_id_xor_space_id", $"({eventSeriesBuilder.GetColumnName(nameof(EventSeries.UserId))} IS NULL AND {eventSeriesBuilder.GetColumnName(nameof(EventSeries.SpaceId))} IS NOT NULL) OR ({eventSeriesBuilder.GetColumnName(nameof(EventSeries.SpaceId))} IS NULL AND {eventSeriesBuilder.GetColumnName(nameof(EventSeries.UserId))} IS NOT NULL)"));

            eventSeriesBuilder.HasKey(eventSeries => eventSeries.Id);

            eventSeriesBuilder.Property(eventSeries => eventSeries.Id).Has(order: 0);
            eventSeriesBuilder.Property(eventSeries => eventSeries.UserId).Has(order: 1);
            eventSeriesBuilder.Property(eventSeries => eventSeries.SpaceId).Has(order: 2);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Handle)!.Has(maxLength: 255, order: 3);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Status).Has(order: 4);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Visibility).Has(order: 5);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Name)!.Has(maxLength: 255, order: 6);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Article).Has(order: 7);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Url).Has(maxLength: 2047, order: 8);
            eventSeriesBuilder.Property(eventSeries => eventSeries.StartsAtNz).Has(order: 9);
            eventSeriesBuilder.Property(eventSeries => eventSeries.Duration).Has(order: 10);
            eventSeriesBuilder.Property(eventSeries => eventSeries.EndsAtNz).Has(order: 11);
            eventSeriesBuilder.Property(eventSeries => eventSeries.RecurrenceRule).Has(maxLength: 255, order: 12);
            eventSeriesBuilder.Property(eventSeries => eventSeries.ExpiresAtNz).Has(order: 13);
            eventSeriesBuilder.Property(eventSeries => eventSeries.CreatedAtUtc).Has(order: 14);
            eventSeriesBuilder.Property(eventSeries => eventSeries.UpdatedAtUtc).Has(order: 15);

            eventSeriesBuilder.HasOneWithMany(eventSeries => eventSeries.User, user => user.EventSeries, eventSeries => eventSeries.UserId, DeleteBehavior.Restrict, required: false);
            eventSeriesBuilder.HasOneWithMany(eventSeries => eventSeries.Space, space => space.EventSeries, eventSeries => eventSeries.SpaceId, DeleteBehavior.Restrict, required: false);
        }
    }
}
