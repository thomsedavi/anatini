using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Contents.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Contents
{
    [ApiController]
    [Route("api/channels/{channelId}/contents")]
    public class ChannelContentsController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostContent(string channelId, [FromForm] CreateContent createContent) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var eventData = new EventData(HttpContext);

            var contentAlias = await context.AddContentAliasAsync(createContent.Id, channel.Id, createContent.Handle, createContent.Name, createContent.Protected);
            var content = await context.AddContentAsync(createContent.Id, createContent.Name, createContent.Handle, createContent.Protected, channel.Id, eventData);

            channel.AddDraftContent(content, eventData);
            await context.UpdateAsync(channel);

            return CreatedAtAction(nameof(GetContent), new { channelId = channel.DefaultHandle, contentId = createContent.Handle }, new { createContent.Id, DefaultHandle = createContent.Handle, createContent.Name });
        }, requiresAccess: true);

        [Authorize]
        [HttpPatch("{contentId}")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchContent(string channelId, string contentId, [FromForm] UpdateContent updateContent) => await UsingContentContextAsync(channelId, contentId, async (content, channel, context) =>
        {
            if (updateContent.DateNZ.HasValue)
            {
                content.DraftVersion.DateNZ = updateContent.DateNZ.Value;
            }

            if (!string.IsNullOrEmpty(updateContent.Name))
            {
                content.DraftVersion.Name = updateContent.Name;
            }

            if (!string.IsNullOrEmpty(updateContent.Article))
            {
                content.DraftVersion.Article = updateContent.Article;
            }

            if (updateContent.Status == "Published")
            {
                if (content.PublishedVersion == null)
                {
                    await context.AddAttributeContent(AttributeContentType.Date, content.DraftVersion.DateNZ.GetDate(), channel, content);
                    await context.AddAttributeContent(AttributeContentType.Week, content.DraftVersion.DateNZ.GetWeek(), channel, content);
                }
                else if (content.PublishedVersion != null && (content.PublishedVersion.DateNZ != content.DraftVersion.DateNZ || content.PublishedVersion.Name != content.DraftVersion.Name || content.PublishedVersion.CardImageId != content.DraftVersion.CardImageId))
                {
                    await context.RemoveAttributeContent(AttributeContentType.Date, content.PublishedVersion.DateNZ.GetDate(), channel, content);
                    await context.RemoveAttributeContent(AttributeContentType.Week, content.PublishedVersion.DateNZ.GetWeek(), channel, content);
                    await context.AddAttributeContent(AttributeContentType.Date, content.DraftVersion.DateNZ.GetDate(), channel, content);
                    await context.AddAttributeContent(AttributeContentType.Week, content.DraftVersion.DateNZ.GetWeek(), channel, content);
                }

                var publishedVersion = new ContentOwnedVersion
                {
                    Name = content.DraftVersion.Name,
                    ContentId = content.Id,
                    ContentChannelId = content.ChannelId,
                    DateNZ = content.DraftVersion.DateNZ,
                    Article = content.DraftVersion.Article
                };

                content.Status = updateContent.Status;
                content.PublishedVersion = publishedVersion;
            }
            else if (updateContent.Status == "Draft")
            {
                if (content.PublishedVersion != null)
                {
                    await context.RemoveAttributeContent(AttributeContentType.Date, content.PublishedVersion.DateNZ.GetDate(), channel, content);
                    await context.RemoveAttributeContent(AttributeContentType.Week, content.PublishedVersion.DateNZ.GetWeek(), channel, content);
                }

                content.Status = updateContent.Status;
                content.PublishedVersion = null;
            }

            await context.UpdateAsync(content);

            return NoContent();
        }, Request.ETagHeader(), refreshETag: true, requiresAccess: true);

        [HttpGet("{contentId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContent(string channelId, string contentId) => await UsingContent(channelId, contentId, content =>
        {
            var versionDto = content.ToContentDto();

            if (versionDto == null)
            {
                return NotFound();
            }

            return Ok(versionDto);
        });

        [HttpGet("{contentId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentEdit(string channelId, string contentId) => await UsingContent(channelId, contentId, content =>
        {
            return Ok(content.ToContentEditDto());
        }, requiresAccess: true);

        [HttpGet("{contentId}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentPreview(string channelId, string contentId) => await UsingContent(channelId, contentId, content =>
        {
            return Ok(content.ToContentDto(usePreview: true));
        }, requiresAccess: true);
    }
}
