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
    [Route("api/spaces/{spaceHandle}/notes")]
    public class SpaceNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes(string spaceHandle, [FromQuery] NotesQuery notesQuery) => await UsingSpaceAsync(spaceHandle, async (noteSpace) =>
        {
            var utcNow = DateTime.UtcNow;

            var notesQueryable = Context.Notes.AsQueryable();

            notesQueryable = notesQueryable
                .AsNoTracking()
                .Where(note => note.SpaceId == noteSpace.Id)
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

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteSpace: noteSpace))));
        });

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote(string spaceHandle, [FromForm] CreateNote createNote) => await UsingSpaceAsync(spaceHandle, async (noteSpace) =>
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

            var note = Context.AddSpaceNoteAsync(createNote.Title, validationResult.SanitizedHtml, createNote.Visibility, noteSpace.Id, Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createNote.Handle));

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { spaceId = noteSpace.Id, noteId = note.Id }, await note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteSpace: noteSpace));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{noteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchNote(string spaceHandle, string noteHandle, [FromForm] UpdateNote updateNote) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, noteSpace) =>
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

            return Ok(await note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteSpace: noteSpace));
        }, new ContextSettings { AccessRequired = true, AsNoTracking = false });

        [HttpGet("{noteHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, noteSpace) =>
        {
            return Ok(await note.ToNoteDtoAsync(UserIdOrNull, BlobService, noteSpace: noteSpace));
        });


        [Authorize]
        [HttpPost("{noteHandle}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsBookmarked);
        });

        [Authorize]
        [HttpPost("{noteHandle}/star")]
        public async Task<IActionResult> PostNoteStar(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/star")]
        public async Task<IActionResult> DeleteNoteStar(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsStarred);
        });

        [Authorize]
        [HttpPost("{noteHandle}/dismiss")]
        public async Task<IActionResult> PostNoteDismiss(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await AddContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsDismissed);
        });

        [Authorize]
        [HttpDelete("{noteHandle}/dismiss")]
        public async Task<IActionResult> DeleteNoteDismiss(string spaceHandle, string noteHandle) => await UsingSpaceContentAsync<Note>(spaceHandle, noteHandle, async (note, _) =>
        {
            return await DeleteContentRelationship(Context, note.Id, UserContentRelationshipLabel.IsDismissed);
        });
    }
}
