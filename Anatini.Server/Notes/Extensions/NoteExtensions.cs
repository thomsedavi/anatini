using Anatini.Server.Channels.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Notes.Extensions
{
    public static class NoteExtensions
    {
        public static async Task<NoteDto> ToNoteDtoAsync(this Note note, string? handle = null, IBlobService? blobService = null)
        {
            return new NoteDto
            {
                Id = note.Id,
                UserHeader = note.User != null ? await note.User.ToUserHeaderDtoAsync(blobService) : null,
                ChannelHeader = note.Channel != null ? await note.Channel.ToChannelHeaderDto(blobService) : null,
                Handle = handle,
                Article = note.Article,
                PublishedAtUtc = note.PublishedAtUtc
            };
        }

        public static NoteEditDto ToNoteEditDto(this Note note, string? handle = null)
        {
            return new NoteEditDto
            {
                Id = note.Id,
                Handle = handle,
                Article = note.Article,
                Visibility = note.Visibility.ToString(),
                PublishedAtUtc = note.PublishedAtUtc
            };
        }
    }
}
