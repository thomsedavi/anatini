using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Notes.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/channels/{channelId}/notes")]
    public class ChannelNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        public async Task<IActionResult> GetNotes(string channelId, DateTime? lastPublishedAtUtc, Guid? lastNoteId, int pageSize = 20) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var notes = context.Notes.AsQueryable();

            notes = notes.AsNoTracking().Where(note => note.ChannelId == channel.Id && note.PublishedAtUtc < DateTime.UtcNow);

            if (IsAuthenticated)
            {
                notes = notes.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                notes = notes.Where(note => note.Visibility == Visibility.Public);
            }

            if (lastPublishedAtUtc.HasValue && lastNoteId.HasValue)
            {
                notes = notes.Where(note => note.PublishedAtUtc < lastPublishedAtUtc.Value || (note.PublishedAtUtc == lastPublishedAtUtc.Value && note.Id < lastNoteId.Value));
            }

            var noteList = await notes.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(pageSize).ToListAsync();

            if (noteList == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(noteList.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        });

        [Authorize]
        [HttpPost("{noteId}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string channelId, string noteId) => await UsingChannelNoteContextAsync(channelId, noteId, async (note, context) =>
        {
            return Ok();
        });

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

            var note = context.AddChannelNoteAsync(validationResult.SanitizedHtml, createNote.Visibility, channel.Id, Status.Published, DateTime.UtcNow, createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null);

            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { channelId = channel.Id, noteId = note.Id }, await note.ToNoteDtoAsync(createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchNote(string channelId, string noteId, [FromForm] UpdateNote updateNote) => await UsingChannelNoteContextAsync(channelId, noteId, async (note, context) =>
        {
            if (updateNote.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updateNote.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                note.Article = validationResult.SanitizedHtml;
            }

            if (updateNote.PublishedAtNz.HasValue)
            {
                var timeZoneInfoNZ = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                note.PublishedAtUtc = TimeZoneInfo.ConvertTimeToUtc(updateNote.PublishedAtNz.Value, timeZoneInfoNZ);
            }

            note.UpdatedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(note.ToNoteEditDto());
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [HttpGet("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string channelId, string noteId) => await UsingChannelNoteAsync(channelId, noteId, async (note) =>
        {
            return Ok(await note.ToNoteDtoAsync(noteId, BlobService));
        });

        [Authorize]
        [HttpGet("{noteId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNoteEdit(string channelId, string noteId) => await UsingChannelNoteAsync(channelId, noteId, async (note) =>
        {
            return Ok(note.ToNoteEditDto());
        }, new ContextSettings { AccessRequired = true });
    }
}
