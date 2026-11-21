using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> PostNote(string userHandle, [FromForm] CreateNote createNote) => await UsingUserAsync(userHandle, async (noteUser) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createNote.Body);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var note = Context.AddUserNoteAsync(validationResult.SanitizedHtml, createNote.Visibility, noteUser.Id, createNote.Status ?? Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createNote.Handle), createNote.PublishedAtNz);

            await Context.SaveChangesAsync();

            note.User = noteUser;

            return CreatedAtAction(nameof(GetNote), new { userHandle = noteUser.Handle, noteHandle = note.Handle }, await note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteUser: noteUser));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes(string userHandle, [FromQuery] NotesQuery notesQuery) => await UsingUserAsync(userHandle, async (noteUser) =>
        {
            var utcNow = DateTime.UtcNow;

            var notesQueryable = Context.Notes.AsQueryable();

            notesQueryable = notesQueryable
                .AsNoTracking()
                .Where(note => note.UserId == noteUser.Id)
                .Where(note => note.PublishedAtUtc < utcNow);

            if (TryGetUserId(out Guid sourceUserId))
            {
                notesQueryable = notesQueryable.Include(note => note.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));

                notesQueryable = notesQueryable.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                ApplyQueryFilters(notesQueryable, notesQuery, sourceUserId);
            }
            else
            {
                notesQueryable = notesQueryable.Where(note => note.Visibility == Visibility.Public);
            }

            if (notesQuery.LastPublishedAtNz.HasValue && notesQuery.LastNoteId.HasValue)
            {
                notesQueryable = notesQueryable.Where(note => note.PublishedAtUtc < notesQuery.LastPublishedAtNz.Value.ConvertNzToUtc() || (note.PublishedAtUtc == notesQuery.LastPublishedAtNz.Value.ConvertNzToUtc() && note.Id < notesQuery.LastNoteId.Value));
            }

            var notes = await notesQueryable.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(notesQuery.PageSize ?? 10).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteUser: noteUser))));
        });

        [Authorize]
        [HttpPatch("{noteHandle}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchNote(string userHandle, string noteHandle, [FromForm] UpdateNote updateNote) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            if (updateNote.Body != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updateNote.Body);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                note.Body = validationResult.SanitizedHtml;
            }

            if (updateNote.PublishedAtNz.HasValue)
            {
                note.PublishedAtUtc = updateNote.PublishedAtNz.Value.ConvertNzToUtc();
            }

            note.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return NoContent();
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [HttpGet("{noteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, noteUser) =>
        {
            return Ok(await note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteUser: noteUser));
        });

        [Authorize]
        [HttpPost("{noteHandle}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{noteHandle}/star")]
        public async Task<IActionResult> PostNoteStar(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/star")]
        public async Task<IActionResult> DeleteNoteStar(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{noteHandle}/dismiss")]
        public async Task<IActionResult> PostNoteDismiss(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/dismiss")]
        public async Task<IActionResult> DeleteNoteDismiss(string userHandle, string noteHandle) => await UsingUserContentAsync<Note>(userHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsDismissed);
        });
    }
}
