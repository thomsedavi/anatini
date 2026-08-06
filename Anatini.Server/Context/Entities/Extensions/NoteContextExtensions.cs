using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Post AddUserNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var noteId = Guid.CreateVersion7();

            var publishedatUtc = utcNow;

            if (publishedAtNZ.HasValue)
            {
                publishedatUtc = publishedAtNZ.Value.ConvertNzToUtc();
            }

            var note = new Post
            {
                Id = noteId,
                UserId = userId,
                Type = PostType.Note,
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

        public static Post AddSpaceNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid spaceId, Status status, DateTime utcNow, string? handle = null)
        {
            var noteId = Guid.CreateVersion7();

            var note = new Post
            {
                Id = noteId,
                SpaceId = spaceId,
                Type = PostType.Note,
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
