using System.Net.Mime;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Anatini.Server.Users;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Account
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager)
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserEdit() => await UsingAccountAsync(async (user) =>
        {
            return await Task.FromResult(Ok(await user.ToUserEditDto(blobService)));
        });

        [Authorize]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        public async Task<IActionResult> PatchUser([FromForm] UpdateUser updateUser) => await UsingAccountContextAsync(async (user, context) =>
        {
            if (updateUser.Name != null)
            {
                user.Name = updateUser.Name;
            }

            if (Request.Form.Keys.Contains("about"))
            {
                if (updateUser.About != null)
                {
                    var validationResult = HtmlContentService.ValidateAndNormalizeHtml(updateUser.About);

                    if (validationResult.ErrorMessage != null)
                    {
                        return BadRequest(new { error = validationResult.ErrorMessage });
                    }
                    else if (validationResult.SanitizedHtml == null)
                    {
                        return BadRequest(new { error = "Unknown error" });
                    }

                    user.About = validationResult.SanitizedHtml;
                }
                else
                {
                    user.About = null;
                }
            }

            if (updateUser.Visibility.HasValue)
            {
                user.Visibility = updateUser.Visibility.Value;
            }

            await context.SaveChangesAsync();

            return NoContent();
        }, new ContextSettings { AsNoTracking = false });

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
        public async Task<IActionResult> PostImage([FromForm] CreateImage createImage) => await UsingAccountContextAsync(async (user, context) =>
        {
            if (ImageValidationError(createImage, out ActionResult? issue))
            {
                return issue ?? BadRequest();
            }

            var imageId = Guid.CreateVersion7();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            context.AddUserImage(imageId, user.Id, NormalizeHandle(createImage.Handle), blobContainerName, blobName, createImage.AltText);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(UsersController.GetImage), "Users", new { userId = user.Id, imageId }, new { Id = imageId, UserId = user.Id });
        });

        // TODO when filtering logs by date, ensure date time format like new DateTime(2026, 2, 19, 0, 0, 0, DateTimeKind.Utc);
    }
}
