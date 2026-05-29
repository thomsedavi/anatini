using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Note AddUserNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var noteId = Guid.CreateVersion7();

            var publishedatUtc = utcNow;

            if (publishedAtNZ.HasValue)
            {
                var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                publishedatUtc = TimeZoneInfo.ConvertTimeToUtc(publishedAtNZ.Value, timeZoneInfoNZ);
            }

            var note = new Note
            {
                Id = noteId,
                UserId = userId,
                Handle = handle ?? noteId.ToString(),
                PublishedAtUtc = publishedatUtc.Truncate(),
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

        public static Note AddChannelNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid channelId, Status status, DateTime utcNow, string? handle = null)
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
