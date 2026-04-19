using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : AnatiniControllerBase(context, userManager)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNotes() => await UsingContextAsync(async (context) =>
        {
            return Ok();
        });
    }
}
