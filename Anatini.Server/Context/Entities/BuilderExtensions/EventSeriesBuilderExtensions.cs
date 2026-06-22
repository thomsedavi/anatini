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

            eventSeriesBuilder.HasOneWithMany(eventSeries => eventSeries.User, user => user.EventSeries, eventSeries => eventSeries.UserId, DeleteBehavior.Restrict, required: false);
            eventSeriesBuilder.HasOneWithMany(eventSeries => eventSeries.Space, space => space.EventSeries, eventSeries => eventSeries.SpaceId, DeleteBehavior.Restrict, required: false);
        }
    }
}
