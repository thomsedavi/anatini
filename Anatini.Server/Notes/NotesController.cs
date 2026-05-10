using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Notes.Extensions;
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
        public async Task<IActionResult> GetNotes(DateTime? lastPublishedAt, Guid? lastNoteId, int pageSize = 20) => await UsingContextAsync(async (context) =>
        {
            var notes = context.Notes.AsQueryable();

            notes = notes.AsNoTracking().Where(note => note.PublishedAtUtc < DateTime.UtcNow);
            notes = notes.Include(note => note.User).ThenInclude(user => user!.Images);
            notes = notes.Include(note => note.Channel).ThenInclude(channel => channel!.Images);

            if (IsAuthenticated)
            {
                notes = notes.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);
            }
            else
            {
                notes = notes.Where(note => note.Visibility == Visibility.Public);
            }

            if (lastPublishedAt.HasValue && lastNoteId.HasValue)
            {
                notes = notes.Where(note => note.PublishedAtUtc < lastPublishedAt.Value || (note.PublishedAtUtc == lastPublishedAt.Value && note.Id < lastNoteId.Value));
            }

            var noteList = await notes.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(pageSize).ToListAsync();

            if (noteList == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(noteList.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        });
    }
}
