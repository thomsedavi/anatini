using Anatini.Server.Enums;
using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Note AddUserNoteAsync(this ApplicationDbContext context, string? title, string body, Visibility visibility, Guid userId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var noteId = Guid.CreateVersion7();
        
            var publishedatUtc = utcNow;
        
            if (publishedAtNZ.HasValue)
            {
                publishedatUtc = publishedAtNZ.Value.ConvertNzToUtc();
            }

            var note = new Note
            {
                Id = noteId,
                UserId = userId,
                Type = ContentType.Note,
                Handle = handle ?? noteId.ToString(),
                PublishedAtUtc = publishedatUtc.Truncate(),
                TimeZoneId = "Pacific/Auckland",
                Title = title,
                Body = body,
                Visibility = visibility,
                Status = status,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };
        
            context.Add(note);
        
            return note;
        }
        
        public static Note AddSpaceNoteAsync(this ApplicationDbContext context, string? title, string body, Visibility visibility, Guid spaceId, Status status, DateTime utcNow, string? handle = null, DateTime? publishedAtNZ = null)
        {
            var noteId = Guid.CreateVersion7();
        
            var publishedatUtc = utcNow;
        
            if (publishedAtNZ.HasValue)
            {
                publishedatUtc = publishedAtNZ.Value.ConvertNzToUtc();
            }
        
            var note = new Note
            {
                Id = noteId,
                SpaceId = spaceId,
                Type = ContentType.Note,
                Handle = handle ?? noteId.ToString(),
                PublishedAtUtc = publishedatUtc.Truncate(),
                TimeZoneId = "Pacific/Auckland",
                Title = title,
                Body = body,
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
