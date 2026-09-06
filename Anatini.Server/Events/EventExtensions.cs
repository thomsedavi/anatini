using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;

namespace Anatini.Server.Events
{
    public static class EventExtensions
    {
        public static EventOccurrenceDto ToEventSeriesDto(this EventSeries eventSeries)
        {
            return new EventOccurrenceDto
            {
                Id = eventSeries.Id,
                Name = eventSeries.Name ?? throw new InvalidOperationException()
            };
        }

        public static EventDto ToEventInstanceDto(this EventInstance eventInstance, bool isAuthenticated)
        {
            return new EventDto
            {
                Id = eventInstance.Id,
                Handle = eventInstance.Handle,
                StartsAtNz = eventInstance.StartsAtNz,
                EndsAtNz = eventInstance.EndsAtNz,
                Name = eventInstance.Name,
                Article = eventInstance.Article,
                Url = eventInstance.Url,
                HasBookmarked = isAuthenticated ? eventInstance.UserRelationships.Any(userRelationship => userRelationship.Label == UserEventInstanceRelationshipLabel.HasBookmarked) : null
            };
        }
    }
}
