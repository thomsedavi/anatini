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
                Article = note.Article
            };
        }
    }
}
