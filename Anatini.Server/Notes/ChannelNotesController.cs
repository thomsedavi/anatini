using System.Net.Mime;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Notes.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/channels/{channelId}/notes")]
    public class ChannelNotesController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote(string channelId, [FromForm] CreateNote createNote) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var eventData = new EventData(HttpContext);

            var note = await context.AddNoteAsync(createNote.Id, createNote.Article, createNote.Protected, channel.Id, eventData);

            return CreatedAtAction(nameof(GetNote), new { channelId = channel.DefaultHandle, noteId = note.Id }, note.ToNoteDto());
        }, requiresAccess: true);

        [HttpGet("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string channelId, string noteId) => await UsingNote(channelId, noteId, note =>
        {
            return Ok(note.ToNoteDto());
        });
    }
}
