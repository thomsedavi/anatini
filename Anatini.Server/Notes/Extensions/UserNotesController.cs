using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            return Ok();
        });
    }
}
