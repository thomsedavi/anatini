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
    [Route("api/users/{userId}/notes")]
    public class UserNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("{noteId}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await AddUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [Authorize]
        [HttpDelete("{noteId}/bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await DeleteUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{noteId}/star")]
        public async Task<IActionResult> PostNoteStar(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await AddUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{noteId}/star")]
        public async Task<IActionResult> DeleteNoteStar(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await DeleteUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{noteId}/seen")]
        public async Task<IActionResult> PostNoteSeen(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await AddUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasSeen);
        });

        [Authorize]
        [HttpDelete("{noteId}/seen")]
        public async Task<IActionResult> DeleteNoteSeen(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            return await DeleteUserNoteEdge(context, note.Id, UserNoteEdgeLabel.HasSeen);
        });

        private async Task<IActionResult> AddUserNoteEdge(ApplicationDbContext context, Guid noteId, UserNoteEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = new ApplicationUserNoteEdge
                {
                    SourceUserId = sourceUserId,
                    TargetNoteId = noteId,
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
                var userNoteEdge = await context.UserNoteEdges.FirstOrDefaultAsync(userNoteEdge => userNoteEdge.TargetNoteId == noteId && userNoteEdge.SourceUserId == sourceUserId && userNoteEdge.Label == label);

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
