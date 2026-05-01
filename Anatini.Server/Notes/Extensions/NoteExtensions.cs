using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;

namespace Anatini.Server.Notes.Extensions
{
    public static class NoteExtensions
    {
        public static NoteDto ToNoteDto(this Note note)
        {
            return new NoteDto
            {
                Id = note.Id,
                Article = note.Article,
                PublishedAtUtc = note.PublishedAtUtc
            };
        }

        public static NoteEditDto ToNoteEditDto(this Note note)
        {
            return new NoteEditDto
            {
                Id = note.Id,
                Article = note.Article
            };
        }
    }
}
