using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Notes.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Notes
{
    [ApiController]
    [Route("api/account/notes")]
    public class AccountNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : AnatiniControllerBase(context, userManager)
    {
        [Authorize]
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostNote([FromForm] CreateNote createNote) => await UsingAccountContextAsync(async (user, context) =>
        {
            var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createNote.Article);

            if (validationResult.ErrorMessage != null)
            {
                return BadRequest(new { error = validationResult.ErrorMessage });
            }
            else if (validationResult.SanitizedHtml == null)
            {
                return BadRequest(new { error = "Unknown error" });
            }

            var note = context.AddUserNoteAsync(validationResult.SanitizedHtml, createNote.Visibility, user.Id, PostStatus.Published, DateTime.UtcNow, createNote.Handle != null ? NormalizeHandle(createNote.Handle) : null);

            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { noteId = note.Id }, note.ToNoteDto());
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{noteId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNote(string noteId) => await UsingAccountNoteAsync(noteId, async (note) =>
        {
            return Ok(note.ToNoteDto());
        });
    }
}
