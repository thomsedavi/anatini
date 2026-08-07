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
using Npgsql;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/users/{userHandle}/notes")]
    public class UserNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote([FromForm] CreateNote createNote) => await UsingAccountAsync(async (user) =>
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

            note.User = user;

            return CreatedAtAction(nameof(GetNote), new { userHandle = user.Handle, noteHandle = note.Handle }, await note.ToNoteDtoAsync(createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes(DateTime? lastPublishedAtNz, Guid? lastNoteId, int pageSize = 20) => await UsingAccountAsync(async (user) =>
        {
            var notesQuery = Context.Notes;

            notesQuery = notesQuery.AsNoTracking();

            notesQuery = notesQuery.Where(note => note.UserId == user.Id);

            if (lastPublishedAtNz.HasValue && lastNoteId.HasValue)
            {
                notesQuery = notesQuery.Where(note => note.PublishedAtNz < lastPublishedAtNz.Value || (note.PublishedAtNz == lastPublishedAtNz.Value && note.Id < lastNoteId.Value));
            }

            var notes = await notesQuery.OrderByDescending(note => note.PublishedAtNz).ThenByDescending(note => note.Id).Take(pageSize).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        });

        [Authorize]
        [HttpGet("{noteHandle}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNoteEdit(string noteHandle) => await UsingAccountNoteAsync(noteHandle, async (note) =>
        {
            return Ok(note.ToNoteEditDto(noteHandle));
        });

        [Authorize]
        [HttpPatch("{noteHandle}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchNote(string noteHandle, [FromForm] UpdateNote updateNote) => await UsingAccountNoteAsync(noteHandle, async (note) =>
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
                note.PublishedAtNz = updateNote.PublishedAtNz.Value;
            }

            note.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return Ok(note.ToNoteEditDto());
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [Authorize]
        [HttpGet("{noteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string noteHandle) => await UsingAccountNoteAsync(noteHandle, async (note) =>
        {
            return Ok(await note.ToNoteDtoAsync(noteHandle, BlobService));
        });

        [Authorize]
        [HttpPost("{noteHandle}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{noteHandle}/star")]
        public async Task<IActionResult> PostNoteStar(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/star")]
        public async Task<IActionResult> DeleteNoteStar(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{noteHandle}/dismiss")]
        public async Task<IActionResult> PostNoteDismiss(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/dismiss")]
        public async Task<IActionResult> DeleteNoteDismiss(string userHandle, string noteHandle) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserNoteEdge(ApplicationDbContext context, Guid noteId, UserNoteEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = new ApplicationUserPostEdge
                {
                    SourceUserId = sourceUserId,
                    TargetPostId = noteId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userNoteEdge);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                }

                return Created();
            }
            else
            {
                return Problem();
            }
        }

        private async Task<IActionResult> DeleteUserNoteEdge(ApplicationDbContext context, Guid noteId, UserNoteEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = await context.UserPostEdges.FirstOrDefaultAsync(userNoteEdge => userNoteEdge.TargetPostId == noteId && userNoteEdge.SourceUserId == sourceUserId && userNoteEdge.Label == label);

                if (userNoteEdge != null)
                {
                    context.Remove(userNoteEdge);
                    await context.SaveChangesAsync();
                }

                return NoContent();
            }
            else
            {
                return Problem();
            }
        }
    }
}
