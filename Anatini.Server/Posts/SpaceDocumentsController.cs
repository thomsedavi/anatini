using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/spaces/{spaceHandle}/documents")]
    public class SpaceDocumentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostDocument(string spaceHandle, [FromForm] CreateDocument createDocument) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            var eventData = new EventData(HttpContext);

            var documentId = Guid.CreateVersion7();

            Context.AddDocument(documentId, createDocument.Name, createDocument.Handle, space.Id);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDocument), new { spaceId = space.Id, documentId = createDocument.Handle }, new { documentId, DefaultHandle = createDocument.Handle, createDocument.Name });
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{documentHandle}")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchDocument(string spaceHandle, string documentHandle, [FromForm] UpdateDocument updateDocument) => await UsingSpacePostAsync(spaceHandle, documentHandle, PostType.Document, async (document) =>
        {
            return NoContent();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{documentHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocument(string spaceHandle, string documentHandle) => await UsingSpacePostAsync(spaceHandle, documentHandle, PostType.Document, async (document) =>
        {
            return Ok();
        });

        [HttpGet("{documentHandle}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentEdit(string spaceHandle, string documentHandle) => await UsingSpacePostAsync(spaceHandle, documentHandle, PostType.Document, async (document) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{documentHandle}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentPreview(string spaceHandle, string documentHandle) => await UsingSpacePostAsync(spaceHandle, documentHandle, PostType.Document, async (document) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });
    }
}
