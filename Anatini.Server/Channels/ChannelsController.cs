using System.Diagnostics;
using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Common;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/channels")]
    public class ChannelsController(IBlobService blobService) : AnatiniControllerBase
    {
        [HttpGet("{channelId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannel(string channelId) => await UsingChannel(channelId, channel =>
        {
            return Ok(channel.ToChannelDto());
        });

        [Authorize]
        [HttpGet("{channelId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannelEdit(string channelId) => await UsingChannel(channelId, channel =>
        {
            return Ok(channel.ToChannelEditDto());
        }, requiresAuthorisation: true);

        [Authorize]
        [HttpPost("{channelId}/images")]
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
        public async Task<IActionResult> PostImage(string channelId, [FromForm] CreateImage createImage) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            if (createImage.File == null || createImage.File.Length == 0)
            {
                return BadRequest();
            }

            if (!Enum.TryParse(createImage.Type, out ImageType imageType))
            {
                return ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            var extension = Path.GetExtension(createImage.File.FileName).ToLowerInvariant();

            if (extension != ".jpg" && extension != ".jpeg")
            {
                return ValidationProblem(statusCode: StatusCodes.Status415UnsupportedMediaType);
            }

            if (createImage.File.Length > 1024 * 1024)
            {
                return ValidationProblem(statusCode: StatusCodes.Status413PayloadTooLarge);
            }

            var (width, height) = imageType switch
            {
                ImageType.Card => (480, 360),
                _ => throw new UnreachableException()
            };

            var result = createImage.File.GetJpegDimensions();

            if (result?.Width != width && result?.Height != height)
            {
                return ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            var imageId = Guid.NewGuid();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            await context.AddChannelImageAsync(imageId, channel.Id, blobContainerName, blobName);

            return CreatedAtAction(nameof(GetImage), new { channelId, imageId }, new { Id = imageId, ChannelId = channel.Id });
        }, requiresAuthorisation: true);

        [Authorize]
        [HttpGet("{channelId}/images/{imageId}")]
        public async Task<IActionResult> GetImage(string channelId, string imageId) => await UsingChannelAliasAsync(channelId, async channelAlias =>
        {
            return await Task.FromResult(Ok($"TODO Image Result for {imageId}"));
        });

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostChannel([FromForm] NewChannel newChannel) => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            await context.AddChannelAliasAsync(newChannel.Slug, newChannel.Id, newChannel.Name, newChannel.Protected);
            var channel = await context.AddChannelAsync(newChannel.Id, newChannel.Name, newChannel.Slug, newChannel.Protected, user.Id, user.Name);

            user.AddChannel(channel);
            await context.Update(user);

            return Ok(user.ToUserEditDto());
        });
    }
}
