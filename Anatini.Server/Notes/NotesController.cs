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
        public async Task<IActionResult> GetNotes([FromQuery] NotesQuery query)
        {
            var utcNow = DateTime.UtcNow;

            var notesQuery = Context.Notes.AsQueryable();

            notesQuery = notesQuery
                .AsNoTracking()
                .Where(note => note.PublishedAtUtc < utcNow)
                .Include(note => note.User).ThenInclude(user => user!.Images)
                .Include(note => note.Space).ThenInclude(space => space!.Images);

            if (TryGetUserId(out Guid sourceUserId))
            {
                notesQuery = notesQuery
                    .Include(note => note.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId))
                    .Include(note => note.Space).ThenInclude(space => space!.UserRelationships.Where(userSpaceRelationship => userSpaceRelationship.SourceUserId == sourceUserId));

                notesQuery = notesQuery.Where(note => (note.Visibility & (Visibility.Public | Visibility.Protected)) != 0);

                if (query.Bookmarked == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }
                else if (query.Bookmarked == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
                }

                if (query.Starred == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }
                else if (query.Starred == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
                }

                if (query.Dismissed == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }
                else if (query.Dismissed != "show")
                {
                    notesQuery = notesQuery.Where(note => !note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
                }

                if (query.Collected == "only")
                {
                    notesQuery = notesQuery.Where(note => note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }
                else if (query.Collected == "hide")
                {
                    notesQuery = notesQuery.Where(note => !note.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
                }

                if (query.Followed == "only")
                {
                    notesQuery = notesQuery.Where(note => note.User != null && note.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
                else if (query.Followed == "hide")
                {
                    notesQuery = notesQuery.Where(note => note.User != null && !note.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
                }
            }
            else
            {
                notesQuery = notesQuery.Where(note => note.Visibility == Visibility.Public);
            }

            if (query.LastPublishedAtNz.HasValue && query.LastNoteId.HasValue)
            {
                notesQuery = notesQuery.Where(note => note.PublishedAtUtc < query.LastPublishedAtNz.Value.ConvertNzToUtc() || (note.PublishedAtUtc == query.LastPublishedAtNz.Value.ConvertNzToUtc() && note.Id < query.LastNoteId.Value));
            }

            var notes = await notesQuery.OrderByDescending(note => note.PublishedAtUtc).ThenByDescending(note => note.Id).Take(query.PageSize ?? 10).ToListAsync();

            if (notes == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(notes.Select(note => note.ToNoteDtoAsync(UserIdOrNull, BlobService))));
        }
    }
}
