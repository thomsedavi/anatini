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
        public async Task<IActionResult> GetNotes([FromQuery] NotesQuery query) => await UsingContextAsync(async () =>
        {
            var notesQuery = Context.Notes;

            notesQuery = notesQuery.AsNoTracking().Where(note => note.PublishedAtUtc < DateTime.UtcNow);
            notesQuery = notesQuery.Include(note => note.User).ThenInclude(user => user!.Images);
            notesQuery = notesQuery.Include(note => note.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid userId))
            {
                notesQuery = notesQuery.Include(note => note.UserEdges.Where(userNote => userNote.SourceUserId == userId));

                notesQuery = notesQuery.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasBookmarked));
                }

                if (query.Starred == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasStarred));
                }
                else if (query.Starred == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasStarred));
                }

                if (query.Dismissed == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserNoteEdgeLabel.HasDismissed));
                }

                if (query.Followed == "only")
                {
                    notesQuery = notesQuery.Where(note => note.User != null && note.User.ReceivedUserEdges.Any(userEdge => userEdge.SourceUserId == userId && userEdge.Label == UserUserEdgeLabel.HasFollowed));
                }
                else if (query.Followed == "hide")
                {
                    notesQuery = notesQuery.Where(note => note.User != null && !note.User.ReceivedUserEdges.Any(test => test.SourceUserId == userId && test.Label == UserUserEdgeLabel.HasFollowed));
                }
            }
            else
            {
                notesQuery = notesQuery.Where(note => note.Visibility == Visibility.Public);
            }

            if (query.LastPublishedAtUtc.HasValue && query.LastNoteId.HasValue)
            {
                notesQuery = notesQuery.Where(note => note.PublishedAtUtc < query.LastPublishedAtUtc.Value || (note.PublishedAtUtc == query.LastPublishedAtUtc.Value && note.Id < query.LastNoteId.Value));
            }

            var notes = await notesQuery.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        });

        public class NotesQuery
        {
            public DateTime? LastPublishedAtUtc { get; set; }
            public Guid? LastNoteId { get; set; }
            public int? PageSize { get; set; }
            public string? Bookmarked { get; set; }
            public string? Starred { get; set; }
            public string? Dismissed { get; set; }
            public string? Followed { get; set; }
        }
    }
}
