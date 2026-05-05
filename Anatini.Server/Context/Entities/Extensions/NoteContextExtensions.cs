using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Note AddUserNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid userId, PostStatus status, DateTime utcNow, string? handle = null)
        {
            var noteId = Guid.CreateVersion7();

            var note = new Note
            {
                Id = noteId,
                UserId = userId,
                Handle = handle ?? noteId.ToString(),
                PublishedAtUtc = utcNow.Truncate(),
                Article = article,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(note);

            return note;
        }

        public static Note AddChannelNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid channelId, PostStatus status, DateTime utcNow, string? handle = null)
        {
            var noteId = Guid.CreateVersion7();

            var note = new Note
            {
                Id = noteId,
                ChannelId = channelId,
                Handle = handle ?? noteId.ToString(),
                PublishedAtUtc = utcNow.Truncate(),
                Article = article,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(note);

            return note;
        }
    }
}
