using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes([FromQuery] NotesQuery notesQuery)
        {
            var utcNow = DateTime.UtcNow;

            var notesQueryable = Context.Notes.AsQueryable();

            notesQueryable = notesQueryable
                .AsNoTracking()
                .Where(note => note.PublishedAtUtc < utcNow)
                .Include(note => note.User).ThenInclude(user => user!.Images)
                .Include(note => note.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                notesQueryable = notesQueryable
                    .Include(note => note.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId))
                    .Include(note => note.Space).ThenInclude(space => space!.UserRelationships.Where(userSpaceRelationship => userSpaceRelationship.SourceUserId == sourceUserId));

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

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(UserIdOrNull, BlobService))));
        }
    }
}
