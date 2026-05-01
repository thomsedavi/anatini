using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Note AddNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid channelId, PostStatus status, DateTime utcNow, string? handle = null)
        {
            var noteId = Guid.CreateVersion7();

            var channelNote = new ChannelNote
            {
                ChannelId = channelId,
                Handle = handle ?? noteId.ToString(),
                NoteId = noteId,
                CreatedAtUtc = utcNow
            };

            var note = new Note
            {
                Id = noteId,
                PublishedAtUtc = utcNow.Truncate(),
                Article = article,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                ChannelNotes = [channelNote]
            };

            context.Add(note);

            return note;
        }
    }
}
