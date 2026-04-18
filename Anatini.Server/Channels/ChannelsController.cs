using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Channels
{
    [ApiController]
    [Route("api/channels")]
    public class ChannelsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager)
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannelEdit(string channelId) => await UsingChannel(channelId, channel =>
        {
            return Ok(channel.ToChannelEditDto());
        }, requiresAccess: true);

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
            if (updateChannel.Name != null)
            {
                channel.Name = updateChannel.Name;

                foreach (var alias in channel.Aliases)
                {
                    //var channelAlias = await context.Context.ChannelAliases.FindAsync(alias.Handle);
                    //
                    //if (channelAlias != null)
                    //{
                    //    channelAlias.ChannelName = updateChannel.Name;
                    //    await context.UpdateAsync(channelAlias);
                    //}
                }
            }

            if (updateChannel.DefaultCardImageId != null)
            {
                channel.DefaultCardImageId = updateChannel.DefaultCardImageId;

                foreach (var alias in channel.Aliases)
                {
                    //var channelAlias = await context.Context.ChannelAliases.FindAsync(alias.Handle);
                    //
                    //if (channelAlias != null)
                    //{
                    //    channelAlias.DefaultCardImageId = updateChannel.DefaultCardImageId;
                    //    await context.UpdateAsync(channelAlias);
                    //}
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

            var imageId = Guid.CreateVersion7();

            var blobContainerName = "anatini-dev";
            var blobName = $"{imageId}{Path.GetExtension(createImage.File.FileName)}";

            await blobService.UploadAsync(createImage.File, blobContainerName, blobName);

            context.AddChannelImage(imageId, channel.Id, NormalizeHandle(createImage.Handle), blobContainerName, blobName, createImage.AltText);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImage), new { channelId = channel.Id, imageId }, new { Id = imageId, ChannelId = channel.Id });
        }, requiresAccess: true);

        [Authorize]
        [HttpGet("{channelId}/images/{imageId}")]
        public async Task<IActionResult> GetImage(string channelId, string imageId) => await UsingChannelAliasAsync(channelId, async channelAlias =>
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
        public async Task<IActionResult> PostChannel([FromForm] CreateChannel createChannel) => await UsingContextAsync(async (context) =>
        {
            var channel = context.AddChannel(RequiredUserId, NormalizeHandle(createChannel.Handle), createChannel.Name, createChannel.Visibility);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChannel), new { channelId = createChannel.Handle }, channel.ToChannelDto());
        });
    }
}
