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
    [Route("api/account/notes")]
    public class AccountNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote([FromForm] CreateNote createNote) => await UsingAccountContextAsync(async (user) =>
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

            var note = Context.AddUserNoteAsync(validationResult.SanitizedHtml, createNote.Visibility, user.Id, Status.Published, DateTime.UtcNow, createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null, createNote.PublishedAtNz);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { noteId = note.Id }, await note.ToNoteDtoAsync(createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes(DateTime? lastPublishedAtUtc, Guid? lastNoteId, int pageSize = 20) => await UsingAccountAsync(async (user) =>
        {
            var notesQuery = Context.Notes;

            notesQuery = notesQuery.AsNoTracking();

            notesQuery = notesQuery.Where(note => note.UserId == user.Id);

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
        [HttpGet("{noteId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNoteEdit(string noteId) => await UsingAccountNoteAsync(noteId, async (note) =>
        {
            return Ok(note.ToNoteEditDto(noteId));
        });

        [Authorize]
        [HttpPatch("{noteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchNote(string noteId, [FromForm] UpdateNote updateNote) => await UsingAccountNoteContextAsync(noteId, async (note) =>
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

        [Authorize]
        [HttpGet("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string noteId) => await UsingAccountNoteAsync(noteId, async (note) =>
        {
            return Ok(await note.ToNoteDtoAsync(noteId, BlobService));
        });
    }
}
