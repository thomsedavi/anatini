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
        public static async Task<NoteDto> ToNoteDtoAsync(this Content note, string? handle = null, IBlobService? blobService = null)
        {
            return new NoteDto
            {
                Id = note.Id,
                UserHeader = note.User != null ? await note.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = note.Space != null ? await note.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = handle,
                Article = note.Article ?? "<article></article>",
                PublishedAtUtc = note.PublishedAtUtc,
                HasBookmarked = note.UserEdges.Any(userEdge => userEdge.Label == UserNoteEdgeLabel.HasBookmarked),
                HasDismissed = note.UserEdges.Any(userEdge => userEdge.Label == UserNoteEdgeLabel.HasDismissed),
                HasStarred = note.UserEdges.Any(userEdge => userEdge.Label == UserNoteEdgeLabel.HasStarred)
            };
        }

        public static NoteEditDto ToNoteEditDto(this Content note, string? handle = null)
        {
            return new NoteEditDto
            {
                Id = note.Id,
                Handle = handle,
                Article = note.Article ?? "<article></article>",
                Visibility = note.Visibility.ToString(),
                PublishedAtUtc = note.PublishedAtUtc
            };
        }
    }
}
