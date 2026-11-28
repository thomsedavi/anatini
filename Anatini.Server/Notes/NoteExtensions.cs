using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
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

            var (isBookmarked, isHidden, isLiked, isCollected, isOwned, ownerHeader) = await ContentHelper.GetUserContentDetails(note, userId, noteUser, noteSpace, blobService);

            return new NoteDto
            {
                Id = note.Id,
                OwnerHeader = ownerHeader,
                Handle = note.Handle,
                Title = note.Title,
                Body = note.Body,
                Visibility = note.Visibility.ToString(),
                Status = note.Status.ToString(),
                PublishedAtNz = publishedAtUtc.ConvertUtcToNz(),
                IsScheduled = publishedAtUtc > DateTime.UtcNow ? true : null,
                IsBookmarked = isBookmarked,
                IsHidden = isHidden,
                IsLiked = isLiked,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
