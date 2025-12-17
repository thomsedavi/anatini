using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Contents.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
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
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostContent(string channelId, [FromForm] CreateContent createContent) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var eventData = new EventData(HttpContext);

            var contentAlias = await context.AddContentAliasAsync(createContent.Id, channel.Id, createContent.Slug, createContent.Name, createContent.Protected);
            var content = await context.AddContentAsync(createContent.Id, createContent.Name, createContent.Slug, createContent.Protected, channel.Id, eventData);

            channel.AddDraftContent(content, eventData);
            await context.Update(channel);

            //return CreatedAtAction();
            return Ok(channel.ToChannelEditDto());
        }, requiresAuthorisation: true);

        [Authorize]
        [HttpPut("{contentId}/elements")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutElement(string channelId, string contentId, [FromForm] UpdateElement updateElement) => await UsingContentContextAsync(channelId, contentId, async (content, _, context) =>
        {
            var element = content.DraftVersion.Elements?.FirstOrDefault(element => element.Index == updateElement.Index);

            if (element == null)
            {
                return NotFound();
            }

            element.Content = updateElement.Content;

            await context.Update(content);

            return NoContent();
        }, Request.ETagHeader(), refreshETag: true, requiresAuthorisation: true);

        [Authorize]
        [HttpPost("{contentId}/elements")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostElement(string channelId, string contentId, [FromForm] CreateElement createElement) => await UsingContentContextAsync(channelId, contentId, async (content, _, context) =>
        {
            var elements = content.DraftVersion.Elements ?? [];

            if (elements.Count >= 512)
            {
                return Forbid();
            }

            int? index = null;

            if (elements.Count == 0)
            {
                index = int.MaxValue / 2;
            }
            else
            {
                var nextElements = elements.Where(element => element.Index > createElement.InsertAfter).OrderBy(element => element.Index).ToList();

                int? diff = null;

                if (nextElements.Count == 0)
                {
                    diff = int.MaxValue - createElement.InsertAfter;
                }
                else
                {
                    var nextElement = nextElements.First();
                    diff = nextElement.Index - createElement.InsertAfter;
                }

                if (diff >= 2)
                {
                    index = createElement.InsertAfter + (diff / 2);
                }
            }

            if (!index.HasValue)
            {
                var orderedElements = elements.OrderBy(element => element.Index).ToList();
                var gap = int.MaxValue / (orderedElements.Count + 1);
                var respaceIndex = 1;

                if (createElement.InsertAfter == 0)
                {
                    index = gap * respaceIndex++;
                }

                for (var i = 0; i < orderedElements.Count; i++)
                {
                    if (orderedElements[i].Index == createElement.InsertAfter)
                    {
                        orderedElements[i].Index = gap * respaceIndex++;

                        index = gap * respaceIndex++;
                    }
                    else
                    {
                        orderedElements[i].Index = gap * respaceIndex++;
                    }
                }

                content.DraftVersion.Elements = orderedElements;
            }

            if (!index.HasValue)
            {
                return Problem();
            }

            var element = new ContentOwnedElement
            {
                Index = index.Value,
                Tag = createElement.Tag,
                Content = createElement.Content,
                ContentOwnedVersionContentChannelId = content.ChannelId,
                ContentOwnedVersionContentId = content.Id
            };

            content.DraftVersion.Elements!.Add(element);
            await context.Update(content);

            return await Task.FromResult(CreatedAtAction(nameof(GetContent), new { channelId, contentId }, element.ToContentElementDto()));
        }, Request.ETagHeader(), refreshETag: true, requiresAuthorisation: true);

        [Authorize]
        [HttpPatch("{contentId}")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
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

            if (updateContent.Status == "Published")
            {
                var publishedVersion = new ContentOwnedVersion
                {
                    Name = content.DraftVersion.Name,
                    ContentId = content.Id,
                    ContentChannelId = content.ChannelId,
                    DateNZ = content.DraftVersion.DateNZ
                };

                var draftElements = content.DraftVersion.Elements?.OrderBy(element => element.Index).ToList();

                if (draftElements != null)
                {
                    var publishedElements = new List<ContentOwnedElement>();

                    for (var index = 0; index < draftElements.Count; index++)
                    {
                        var draftElement = draftElements[index];

                        publishedElements.Add(new ContentOwnedElement
                        {
                            Index = index,
                            Tag = draftElement.Tag,
                            Content = draftElement.Content,
                            ContentOwnedVersionContentId = draftElement.ContentOwnedVersionContentId,
                            ContentOwnedVersionContentChannelId = draftElement.ContentOwnedVersionContentChannelId
                        });
                    }

                    publishedVersion.Elements = publishedElements;
                }

                content.Status = updateContent.Status;
                content.PublishedVersion = publishedVersion;

                await context.AddAttributeContent(AttributeContentType.Date, content.DraftVersion.DateNZ.GetDate(), channel, content);
                await context.AddAttributeContent(AttributeContentType.Week, content.DraftVersion.DateNZ.GetWeek(), channel, content);
            }

            await context.Update(content);

            return NoContent();
        }, Request.ETagHeader(), refreshETag: true, requiresAuthorisation: true);

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
        }, requiresAuthorisation: true);

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
        }, requiresAuthorisation: true);
    }
}
