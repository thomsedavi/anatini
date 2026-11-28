using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;

namespace Anatini.Server.Events
{
    public static class EventExtensions
    {
        public static async Task<EventDto> ToEventDto(this Event @event, Guid? userId, ApplicationUser? eventUser = null, Space? eventSpace = null)
        {
            eventUser ??= @event.User;
            eventSpace ??= @event.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned, _) = await ContentHelper.GetUserContentDetails(@event, userId, eventUser, eventSpace);

            return new EventDto
            {
                Id = @event.Id,
                Handle = @event.Handle,
                StartsAtNz = @event.StartsAtUtc.ConvertUtcToNz(),
                EndsAtNz = @event.EndsAtUtc?.ConvertUtcToNz(),
                Title = @event.Title ?? throw new InvalidOperationException(),
                Body = @event.Body,
                Visibility = @event.Visibility.ToString(),
                Status = @event.Status.ToString(),
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }

        public static async Task<EventOccurrenceDto> ToEventOccurrenceDtoAsync(this EventOccurrence eventOccurrence, Guid? userId, IBlobService? blobService = null, ApplicationUser? eventOccurrenceUser = null, Space? eventOccurrenceSpace = null)
        {
            var @event = eventOccurrence.GetEvent();
            eventOccurrenceUser ??= eventOccurrence.User;
            eventOccurrenceSpace ??= eventOccurrence.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned, ownerHeader) = await ContentHelper.GetUserContentDetails(eventOccurrence, userId, eventOccurrenceUser, eventOccurrenceSpace, blobService);

            return new EventOccurrenceDto
            {
                Id = eventOccurrence.Id,
                EventHeader = new EventHeaderDto
                {
                    Id = @event.Id,
                    Handle = @event.Handle,
                    HasRecurrenceRule = @event.RecurrenceRule != null
                },
                OwnerHeader = ownerHeader,
                Handle = eventOccurrence.Handle,
                StartsAtNz = eventOccurrence.StartsAtUtc.ConvertUtcToNz(),
                EndsAtNz = eventOccurrence.EndsAtUtc.ConvertUtcToNz(),
                Title = eventOccurrence.Title ?? throw new InvalidOperationException(),
                Body = eventOccurrence.Body,
                Visibility = eventOccurrence.Visibility.ToString(),
                Status = eventOccurrence.Status.ToString(),
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }

        public static Event GetEvent(this EventOccurrence eventOccurrence)
        {
            return eventOccurrence.Parent as Event ?? throw new InvalidOperationException();
        }
    }
}
