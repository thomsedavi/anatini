using System.Net.Mime;
using Anatini.Server.Common;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Anatini.Server.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Account
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(IBlobService blobService) : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost("images")]
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
        public async Task<IActionResult> PostImage([FromForm] CreateImage createImage) => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            if (ImageValidationError(createImage, out ActionResult? issue))
            {
                return issue ?? BadRequest();
            }

            var imageId = Guid.NewGuid();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            await context.AddUserImageAsync(imageId, user.Id, blobContainerName, blobName);

            return CreatedAtAction(nameof(UsersController.GetImage), "Users", new { userId = user.Id, imageId }, new { Id = imageId, UserId = user.Id });
        });
    }
}
