using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Post AddUserNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var noteId = Guid.CreateVersion7();

            var publishedatNz = utcNow.ConvertUtcToNz();

            if (publishedAtNZ.HasValue)
            {
                publishedatNz = publishedAtNZ.Value;
            }

            var note = new Post
            {
                Id = noteId,
                UserId = userId,
                Type = PostType.Note,
                Handle = handle ?? noteId.ToString(),
                PublishedAtNz = publishedatNz.Truncate(),
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
                PublishedAtNz = utcNow.ConvertUtcToNz().Truncate(),
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
