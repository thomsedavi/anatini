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
    [Route("api/spaces/{spaceId}/notes")]
    public class SpaceNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        public async Task<IActionResult> GetNotes(string spaceId, DateTime? lastPublishedAtUtc, Guid? lastNoteId, int pageSize = 20) => await UsingSpaceAsync(spaceId, async (space) =>
        {
            var notesQuery = Context.Notes;

            notesQuery = notesQuery.AsNoTracking().Where(note => note.SpaceId == space.Id && note.PublishedAtUtc < DateTime.UtcNow);

            if (IsAuthenticated)
            {
                notesQuery = notesQuery.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                notesQuery = notesQuery.Where(note => note.Visibility == Visibility.Public);
            }

            if (lastPublishedAtUtc.HasValue && lastNoteId.HasValue)
            {
                notesQuery = notesQuery.Where(note => note.PublishedAtUtc < lastPublishedAtUtc.Value || (note.PublishedAtUtc == lastPublishedAtUtc.Value && note.Id < lastNoteId.Value));
            }

            var notes = await notesQuery.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(pageSize).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        });

        [Authorize]
        [HttpPost("{noteId}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string spaceId, string noteId) => await UsingSpaceNoteContextAsync(spaceId, noteId, async (note) =>
        {
            return Ok();
        });

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote(string spaceId, [FromForm] CreateNote createNote) => await UsingSpaceContextAsync(spaceId, async (space) =>
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

            var note = Context.AddSpaceNoteAsync(validationResult.SanitizedHtml, createNote.Visibility, space.Id, Status.Published, DateTime.UtcNow, createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { spaceId = space.Id, noteId = note.Id }, await note.ToNoteDtoAsync(createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null, BlobService));
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
        public async Task<IActionResult> PatchNote(string spaceId, string noteId, [FromForm] UpdateNote updateNote) => await UsingSpaceNoteContextAsync(spaceId, noteId, async (note) =>
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
                note.PublishedAtUtc = updateNote.PublishedAtNz.Value.ConvertNzToUtc();
            }

            note.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(note.ToNoteEditDto());
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [HttpGet("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string spaceId, string noteId) => await UsingSpaceNoteAsync(spaceId, noteId, async (note) =>
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
        public async Task<IActionResult> GetNoteEdit(string spaceId, string noteId) => await UsingSpaceNoteAsync(spaceId, noteId, async (note) =>
        {
            return Ok(note.ToNoteEditDto());
        }, new ContextSettings { AccessRequired = true });
    }
}
