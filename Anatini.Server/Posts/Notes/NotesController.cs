using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Posts.Notes.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Posts.Notes
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes([FromQuery] NotesQuery query)
        {
            var notesQuery = Context.Notes;

            notesQuery = notesQuery.AsNoTracking().Where(note => note.PublishedAtNz < DateTime.UtcNow.ConvertUtcToNz());
            notesQuery = notesQuery.Include(note => note.User).ThenInclude(user => user!.Images);
            notesQuery = notesQuery.Include(note => note.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid userId))
            {
                notesQuery = notesQuery.Include(note => note.UserEdges.Where(userNote => userNote.SourceUserId == userId));

                notesQuery = notesQuery.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasBookmarked));
                }

                if (query.Starred == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasStarred));
                }
                else if (query.Starred == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasStarred));
                }

                if (query.Dismissed == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasDismissed));
                }
                else if (query.Dismissed == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserEdges.Any(userNote => userNote.SourceUserId == userId && userNote.Label == UserPostEdgeLabel.HasDismissed));
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

            if (query.LastPublishedAtNz.HasValue && query.LastNoteId.HasValue)
            {
                notesQuery = notesQuery.Where(note => note.PublishedAtNz < query.LastPublishedAtNz.Value || (note.PublishedAtNz == query.LastPublishedAtNz.Value && note.Id < query.LastNoteId.Value));
            }

            var notes = await notesQuery.OrderByDescending(note => note.PublishedAtNz).ThenByDescending(note => note.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(note.Handle, BlobService))));
        }

        public class NotesQuery
        {
            public DateTime? LastPublishedAtNz { get; set; }
            public Guid? LastNoteId { get; set; }
            public int? PageSize { get; set; }
            public string? Bookmarked { get; set; }
            public string? Starred { get; set; }
            public string? Dismissed { get; set; }
            public string? Followed { get; set; }
        }
    }
}
