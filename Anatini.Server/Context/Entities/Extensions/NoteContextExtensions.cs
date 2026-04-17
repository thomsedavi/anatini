using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static Note AddNoteAsync(this ApplicationDbContext context, string article, Visibility visibility, Guid channelId, PostStatus status, DateTime utcNow, string? handle = null)
        {
            var id = Guid.CreateVersion7();

            var note = new Note
            {
                Id = id,
                ChannelId = channelId,
                Handle = handle ?? id.ToString(),
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
