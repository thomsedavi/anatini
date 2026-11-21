using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;

namespace Anatini.Server.Notes
{
    public static class NoteExtensions
    {
        public static async Task<NoteDto> ToNoteDtoAsync(this Note note, Guid? userId, IBlobService? blobService = null, ApplicationUser? noteUser = null, Space? noteSpace = null)
        {
            var publishedAtUtc = note.PublishedAtUtc ?? throw new InvalidOperationException();

            noteUser ??= note.User;
            noteSpace ??= note.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned) = ContentHelper.GetUserContentRelationships(note, userId, noteUser, noteSpace);

            return new NoteDto
            {
                Id = note.Id,
                UserHeader = noteUser != null ? await noteUser.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = noteSpace != null ? await noteSpace.ToSpaceHeaderDto(blobService) : null,
                Handle = note.Handle,
                Title = note.Title,
                Body = note.Body,
                Visibility = note.Visibility.ToString(),
                Status = note.Status.ToString(),
                PublishedAtNz = publishedAtUtc.ConvertUtcToNz(),
                IsScheduled = publishedAtUtc > DateTime.UtcNow ? true : null,
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
