using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Notes.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/channels/{channelId}/notes")]
    public class ChannelNotesController : AnatiniControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotes(string channelId, [FromQuery] GetNotesQuery query) => await UsingContextAsync(async context =>
        {
            //if (!RandomHex.IsX16(channelId))
            //{
            //    var channelAlias = await context.Context.ChannelAliases.FindAsync(channelId);
            //
            //    if (channelAlias == null)
            //    {
            //        return NotFound();
            //    }
            //
            //    channelId = channelAlias.ChannelId.ToString();
            //}
            //
            //var notesPage = await context.Context.Notes.WithPartitionKey(channelId).OrderByDescending(a => a.DateTimeUTC).ToPageAsync(10, query.ContinuationToken);
            //
            //return Ok(new { Notes = notesPage.Values.Select(note => note.ToNoteDto()), notesPage.ContinuationToken });
            return Ok();
        });

        public class GetNotesQuery
        {
            public string? ContinuationToken { get; set; }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote(string channelId, [FromForm] CreateNote createNote) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createNote.Article);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var note = context.AddNoteAsync(validationResult.SanitizedHtml, Visibility.Public, channel.Id, PostStatus.Published, DateTime.UtcNow, createNote.Handle != null ? UserManager.NormalizeName(createNote.Handle) : null);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { channelId = channel.Id, noteId = note.Id }, note.ToNoteDto());
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
