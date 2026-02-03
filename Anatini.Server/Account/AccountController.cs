using System.Net.Mime;
using Anatini.Server.Common;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Anatini.Server.Users;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Account
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(IBlobService blobService) : AnatiniControllerBase
    {
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserEdit() => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            return await Task.FromResult(Ok(await user.ToUserEditDto(context, blobService)));
        });

        [Authorize]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        public async Task<IActionResult> PatchUser([FromForm] UpdateUser updateUser) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            if (updateUser.Name != null)
            {
                user.Name = updateUser.Name;

                foreach (var alias in user.Aliases)
                {
                    var userAlias = await context.Context.UserAliases.FindAsync(alias.Slug);

                    if (userAlias != null)
                    {
                        userAlias.UserName = updateUser.Name;
                        await context.UpdateAsync(userAlias);
                    }
                }
            }

            if (Request.Form.Keys.Contains("about"))
            {
                user.About = updateUser.About;

                foreach (var alias in user.Aliases)
                {
                    var userAlias = await context.Context.UserAliases.FindAsync(alias.Slug);

                    if (userAlias != null)
                    {
                        userAlias.UserAbout = updateUser.About;
                        await context.UpdateAsync(userAlias);
                    }
                }
            }

            if (updateUser.IconImageId != null)
            {
                user.IconImageId = updateUser.IconImageId;

                foreach (var alias in user.Aliases)
                {
                    var userAlias = await context.Context.UserAliases.FindAsync(alias.Slug);

                    if (userAlias != null)
                    {
                        userAlias.IconImageId = updateUser.IconImageId;
                        await context.UpdateAsync(userAlias);
                    }
                }
            }

            if (updateUser.Protected.HasValue)
            {
                user.Protected = updateUser.Protected.Value ? true : null;

                foreach (var alias in user.Aliases)
                {
                    var userAlias = await context.Context.UserAliases.FindAsync(alias.Slug);

                    if (userAlias != null)
                    {
                        userAlias.Protected = updateUser.Protected.Value ? true : null;
                        await context.UpdateAsync(userAlias);
                    }
                }
            }

            await context.UpdateAsync(user);

            return NoContent();
        });

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
        public async Task<IActionResult> PostImage([FromForm] CreateImage createImage) => await UsingUserContextAsync(RequiredUserId, async (user, context) =>
        {
            if (ImageValidationError(createImage, out ActionResult? issue))
            {
                return issue ?? BadRequest();
            }

            var imageId = RandomHex.NextX16();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            await context.AddUserImageAsync(imageId, user.Id, blobContainerName, blobName, createImage.AltText);

            return CreatedAtAction(nameof(UsersController.GetImage), "Users", new { userId = user.Id, imageId }, new { Id = imageId, UserId = user.Id });
        });
    }
}
