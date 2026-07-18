using System.Net.Mime;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Spaces
{
    [ApiController]
    [Route("api/spaces")]
    public class SpacesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpGet("{spaceId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpace(string spaceId) => await UsingSpaceAsync(spaceId, async (space) =>
        {
            return Ok(space.ToSpaceDto());
        });

        [Authorize]
        [HttpGet("{spaceId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpaceEdit(string spaceId) => await UsingSpaceAsync(spaceId, async (space) =>
        {
            return Ok(await space.ToSpaceEditDtoAsync(BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{spaceId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchSpace(string spaceId, [FromForm] UpdateSpace updateSpace) => await UsingSpaceContextAsync(spaceId, async (space) =>
        {
            return NoContent();
        });

        [Authorize]
        [HttpPost("{spaceId}/images")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostImage(string spaceId, [FromForm] CreateImage createImage) => await UsingSpaceContextAsync(spaceId, async (space) =>
        {
            if (ImageValidationError(createImage, out ActionResult? issue))
            {
                return issue ?? BadRequest();
            }

            var normalizedHandle = NormalizeHandle(createImage.Handle);

            var blobContainerName = "anatini-dev";
            var blobName = $"{space.Id}/{normalizedHandle}{Path.GetExtension(createImage.File.FileName)}";

            await BlobService.UploadAsync(createImage.File, blobContainerName, blobName);

            Context.AddSpaceImage(space.Id, normalizedHandle, blobContainerName, blobName, createImage.AltText);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImage), new { spaceId = space.Id, imageId = normalizedHandle }, new { SpaceId = space.Id, ImageId = normalizedHandle });
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet("{spaceId}/images/{imageId}")]
        public async Task<IActionResult> GetImage(string spaceId, string imageId) => await UsingSpaceAsync(spaceId, async (space) =>
        {
            return await Task.FromResult(Ok($"TODO Image Result for {imageId}"));
        });

        [Authorize(Policy = "IsTrusted")]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSpace([FromForm] CreateSpace createSpace) => await UsingAccountContextAsync(async (user) =>
        {
            var space = Context.AddSpace(user.Id, NormalizeHandle(createSpace.Handle), createSpace.Name, createSpace.Visibility);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSpace), new { spaceId = createSpace.Handle }, space.ToSpaceDto());
        });
    }
}
