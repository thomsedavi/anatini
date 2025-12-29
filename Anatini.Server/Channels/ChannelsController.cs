using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Common;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Anatini.Server.Users.Extensions;
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
        [HttpPatch("{channelId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchChannel(string channelId, [FromForm] UpdateChannel updateChannel) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            if (updateChannel.DefaultCardImageId.HasValue)
            {
                channel.DefaultCardImageId = updateChannel.DefaultCardImageId.Value;

                foreach (var alias in channel.Aliases)
                {
                    var channelAlias = await context.Context.ChannelAliases.FindAsync(alias.Slug);

                    if (channelAlias != null)
                    {
                        channelAlias.DefaultCardImageId = updateChannel.DefaultCardImageId.Value;
                        await context.UpdateAsync(channelAlias);
                    }
                }
            }

            await context.UpdateAsync(channel);

            return NoContent();
        });

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
            if (ImageValidationError(createImage, out ActionResult? issue))
            {
                return issue ?? BadRequest();
            }

            var imageId = Guid.NewGuid();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            await context.AddChannelImageAsync(imageId, channel.Id, blobContainerName, blobName);

            return CreatedAtAction(nameof(GetImage), new { channelId = channel.Id, imageId }, new { Id = imageId, ChannelId = channel.Id });
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
        public async Task<IActionResult> PostChannel([FromForm] CreateChannel createChannel) => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            await context.AddChannelAliasAsync(createChannel.Slug, createChannel.Id, createChannel.Name, createChannel.Protected);
            var channel = await context.AddChannelAsync(createChannel.Id, createChannel.Name, createChannel.Slug, createChannel.Protected, user.Id, user.Name);

            user.AddChannel(channel);
            await context.UpdateAsync(user);

            return Ok(user.ToUserEditDto());
        });
    }
}
