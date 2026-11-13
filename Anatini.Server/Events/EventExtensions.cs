using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Events
{
    public static class EventExtensions
    {
        public static EventDto ToEventDto(this Event @event, bool isAuthenticated)
        {
            return new EventDto
            {
                Id = @event.Id,
                Handle = @event.Handle,
                StartsAtNz = @event.StartsAtNz,
                EndsAtNz = @event.EndsAtNz,
                Title = @event.Title ?? throw new InvalidOperationException(),
                Body = @event.Body,
                IsBookmarked = isAuthenticated ? @event.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked) : null,
                IsDismissed = isAuthenticated ? @event.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed) : null,
                IsStarred = isAuthenticated ? @event.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsStarred) : null,
                IsCollected = isAuthenticated ? @event.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsCollected) : null
            };
        }

        public static async Task<EventOccurrenceDto> ToEventOccurrenceDtoAsync(this EventOccurrence eventOccurrence, bool isAuthenticated, IBlobService? blobService = null, ApplicationUser? user = null, Space? space = null)
        {
            user ??= eventOccurrence.User;
            space ??= eventOccurrence.Space;

            return new EventOccurrenceDto
            {
                Id = eventOccurrence.Id,
                EventHeader = new EventHeaderDto
                {
                    Id = eventOccurrence.Event.Id,
                    Handle = eventOccurrence.Event.Handle
                },
                UserHeader = user != null ? await user.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = space != null ? await space.ToSpaceHeaderDto(blobService) : null,
                Handle = eventOccurrence.Handle,
                StartsAtNz = eventOccurrence.StartsAtNz,
                EndsAtNz = eventOccurrence.EndsAtNz,
                Title = eventOccurrence.Title ?? throw new InvalidOperationException(),
                Body = eventOccurrence.Body,
                IsBookmarked = isAuthenticated ? eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked) : null,
                IsDismissed = isAuthenticated ? eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed) : null,
                IsStarred = isAuthenticated ? eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsStarred) : null,
                IsCollected = isAuthenticated ? eventOccurrence.UserRelationships.Any(userContentRelationship => userContentRelationship.Label == UserContentRelationshipLabel.IsCollected) : null
            };
        }
    }
}
