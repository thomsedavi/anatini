using Anatini.Server.Utils;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class NoteContextExtensions
    {
        public static async Task<Note> AddNoteAsync(this AnatiniContext context, string id, string article, bool? @protected, string channelId, EventData eventData)
        {
            var note = new Note
            {
                ItemId = ItemId.Get(channelId, id),
                Id = id,
                ChannelId = channelId,
                Article = article,
                UpdatedDateTimeUTC = eventData.DateTimeUtc,
                Protected = @protected
            };

            await context.AddAsync(note);

            return note;
        }
    }
}
