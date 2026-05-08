using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Notes.Extensions
{
    public static class NoteExtensions
    {
        public static NoteDto ToNoteDto(this Note note, string? handle = null)
        {
            return new NoteDto
            {
                Id = note.Id,
                Handle = handle,
                Article = note.Article,
                PublishedAtUtc = note.PublishedAtUtc
            };
        }

        public static NoteEditDto ToNoteEditDto(this Note note, string? handle = null)
        {
            return new NoteEditDto
            {
                Id = note.Id,
                Handle = handle,
                Article = note.Article,
                Visibility = note.Visibility.ToString(),
                PublishedAtUtc = note.PublishedAtUtc
            };
        }
    }
}
