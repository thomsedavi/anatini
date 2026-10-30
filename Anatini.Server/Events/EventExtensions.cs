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
        public static EventDto ToEventDto(this EventSeries eventSeries, bool isAuthenticated)
        {
            return new EventDto
            {
                Id = eventSeries.Id,
                Handle = eventSeries.Handle,
                StartsAtNz = eventSeries.StartsAtNz,
                EndsAtNz = eventSeries.EndsAtNz,
                Title = eventSeries.Title ?? throw new InvalidOperationException(),
                Body = eventSeries.Body,
                IsBookmarked = isAuthenticated ? eventSeries.UserRelationships.Any(userRelationship => userRelationship.Label == UserContentRelationshipLabel.IsBookmarked) : null
            };
        }

        public static async Task<EventOccurrenceDto> ToEventInstanceDtoAsync(this EventInstance eventInstance, bool isAuthenticated, IBlobService? blobService = null, ApplicationUser? user = null, Space? space = null)
        {
            user ??= eventInstance.User;
            space ??= eventInstance.Space;

            return new EventOccurrenceDto
            {
                Id = eventInstance.Id,
                UserHeader = user != null ? await user.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = space != null ? await space.ToSpaceHeaderDto(blobService) : null,
                Handle = eventInstance.Handle,
                StartsAtNz = eventInstance.StartsAtNz,
                EndsAtNz = eventInstance.EndsAtNz,
                Title = eventInstance.Title,
                Body = eventInstance.Body,
                IsBookmarked = isAuthenticated ? eventInstance.UserRelationships.Any(userRelationship => userRelationship.Label == UserEventInstanceRelationshipLabel.IsBookmarked) : null,
                IsDismissed = isAuthenticated ? eventInstance.UserRelationships.Any(userRelationship => userRelationship.Label == UserEventInstanceRelationshipLabel.IsDismissed) : null,
                IsStarred = isAuthenticated ? eventInstance.UserRelationships.Any(userRelationship => userRelationship.Label == UserEventInstanceRelationshipLabel.IsStarred) : null,
                IsCollected = isAuthenticated ? eventInstance.UserRelationships.Any(userRelationship => userRelationship.Label == UserEventInstanceRelationshipLabel.IsCollected) : null
            };
        }
    }
}
