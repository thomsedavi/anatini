using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Notes.Extensions
{
    public static class NoteExtensions
    {
        public static async Task<NoteDto> ToNoteDtoAsync(this Post note, string? noteHandle = null, IBlobService? blobService = null)
        {
            return new NoteDto
            {
                Id = note.Id,
                UserHeader = note.User != null ? await note.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = note.Space != null ? await note.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = noteHandle,
                Article = note.Article ?? "<article></article>",
                PublishedAtNz = note.PublishedAtNz,
                HasBookmarked = note.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasBookmarked),
                HasDismissed = note.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasDismissed),
                HasStarred = note.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasStarred)
            };
        }

        public static NoteEditDto ToNoteEditDto(this Post note, string? handle = null)
        {
            return new NoteEditDto
            {
                Id = note.Id,
                Handle = handle,
                Article = note.Article ?? "<article></article>",
                Visibility = note.Visibility.ToString(),
                PublishedAtNz = note.PublishedAtNz
            };
        }
    }
}
