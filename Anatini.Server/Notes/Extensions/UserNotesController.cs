using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Notes.Extensions
{
    [ApiController]
    [Route("api/users/{userId}/notes")]
    public class UserNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost("{noteId}/bookmark")]
        public async Task<IActionResult> PostNoteBookmark(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = new ApplicationUserNoteEdge
                {
                    SourceUserId = sourceUserId,
                    TargetNoteId = note.Id,
                    Label = UserNoteEdgeLabel.HasBookmarked,
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
        });

        [Authorize]
        [HttpDelete("{noteId}/bookmark")]
        public async Task<IActionResult> DeleteNoteBookmark(string userId, string noteId) => await UsingUserNoteContextAsync(userId, noteId, async (note, context) =>
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userNoteEdge = await context.UserNoteEdges.FirstOrDefaultAsync(userNoteEdge => userNoteEdge.TargetNoteId == note.Id && userNoteEdge.SourceUserId == sourceUserId && userNoteEdge.Label == UserNoteEdgeLabel.HasBookmarked);

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
        });
    }
}
