using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;

namespace Anatini.Server.Events
{
    public static class EventExtensions
    {
        public static EventDto ToEventDto(this Event @event, Guid? userId, ApplicationUser? eventUser = null, Space? eventSpace = null)
        {
            eventUser ??= @event.User;
            eventSpace ??= @event.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned) = ContentHelper.GetUserContentRelationships(@event, userId, eventUser, eventSpace);

            return new EventDto
            {
                Id = @event.Id,
                Handle = @event.Handle,
                StartsAtNz = @event.StartsAtNz,
                EndsAtNz = @event.EndsAtNz,
                Title = @event.Title ?? throw new InvalidOperationException(),
                Body = @event.Body,
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }

        public static async Task<EventOccurrenceDto> ToEventOccurrenceDtoAsync(this EventOccurrence eventOccurrence, Guid? userId, IBlobService? blobService = null, ApplicationUser? eventOccurrenceUser = null, Space? eventOccurrenceSpace = null)
        {
            eventOccurrenceUser ??= eventOccurrence.User;
            eventOccurrenceSpace ??= eventOccurrence.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned) = ContentHelper.GetUserContentRelationships(eventOccurrence, userId, eventOccurrenceUser, eventOccurrenceSpace);

            return new EventOccurrenceDto
            {
                Id = eventOccurrence.Id,
                EventHeader = new EventHeaderDto
                {
                    Id = eventOccurrence.Event.Id,
                    Handle = eventOccurrence.Event.Handle
                },
                UserHeader = eventOccurrenceUser != null ? await eventOccurrenceUser.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = eventOccurrenceSpace != null ? await eventOccurrenceSpace.ToSpaceHeaderDto(blobService) : null,
                Handle = eventOccurrence.Handle,
                StartsAtNz = eventOccurrence.StartsAtNz,
                EndsAtNz = eventOccurrence.EndsAtNz,
                Title = eventOccurrence.Title ?? throw new InvalidOperationException(),
                Body = eventOccurrence.Body,
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
