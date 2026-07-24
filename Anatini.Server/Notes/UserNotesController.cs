using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/users/{userId}/notes/{noteId}")]
    public class UserNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("star")]
        public async Task<IActionResult> PostNoteStar(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("star")]
        public async Task<IActionResult> DeleteNoteStar(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("dismiss")]
        public async Task<IActionResult> PostNoteDismiss(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await AddUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("dismiss")]
        public async Task<IActionResult> DeleteNoteDismiss(string userId, string noteId) => await UsingUserNoteAsync(userId, noteId, async (note) =>
        {
            return await DeleteUserNoteEdge(Context, note.Id, UserNoteEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserNoteEdge(ApplicationDbContext context, Guid noteId, UserNoteEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = new ApplicationUserContentEdge
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = noteId,
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
                var userNoteEdge = await context.UserContentEdges.FirstOrDefaultAsync(userNoteEdge => userNoteEdge.TargetContentId == noteId && userNoteEdge.SourceUserId == sourceUserId && userNoteEdge.Label == label);

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
