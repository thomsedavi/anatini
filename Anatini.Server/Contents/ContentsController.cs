using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Contents.Extensions;
using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Contents
{
    [ApiController]
    [Route("api/channels/{channelId}/[controller]")]
    public class ContentsController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostContent(string channelId, [FromForm] NewContent newContent)
        {
            async Task<IActionResult> channelContextFunctionAsync(Channel channel, AnatiniContext context)
            {
                var eventData = new EventData(HttpContext);

                var contentAlias = await context.AddContentAliasAsync(newContent.Id, channel.Id, newContent.Slug, newContent.Name);
                var content = await context.AddContentAsync(newContent.Id, newContent.Name, newContent.Slug, channel.Id, eventData);

                channel.AddDraftContent(content, eventData);
                await context.Update(channel);

                //return CreatedAtAction();
                return Ok(channel.ToChannelEditDto());
            }

            return await UsingChannelContextAsync(channelId, channelContextFunctionAsync, requiresAuthorisation: true);
        }

        [Authorize]
        [HttpPost("{contentId}/elements")]
        [ETagRequired]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostElement(string channelId, string contentId, [FromForm] NewElement newElement)
        {
            async Task<IActionResult> contentContextFunctionAsync(Content content, AnatiniContext context)
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
                    var nextElements = elements.Where(element => element.Index > newElement.InsertAfter).OrderBy(element => element.Index).ToList();

                    int? diff = null;

                    if (nextElements.Count == 0)
                    {
                        diff = int.MaxValue - newElement.InsertAfter;
                    }
                    else
                    {
                        var nextElement = nextElements.First();
                        diff = nextElement.Index - newElement.InsertAfter;
                    }

                    if (diff >= 2)
                    {
                        index = newElement.InsertAfter + (diff / 2);
                    }
                }

                if (!index.HasValue)
                {
                    var orderedElements = elements.OrderBy(element => element.Index).ToList();
                    var gap = int.MaxValue / (orderedElements.Count + 1);
                    var respaceIndex = 1;

                    if (newElement.InsertAfter == 0)
                    {
                        index = gap * respaceIndex++;
                    }

                    for (var i = 0; i < orderedElements.Count; i++)
                    {
                        if (orderedElements[i].Index == newElement.InsertAfter)
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
                    Tag = newElement.Tag,
                    Content = newElement.Content,
                    ContentOwnedVersionContentChannelId = content.ChannelId,
                    ContentOwnedVersionContentId = content.Id
                };

                content.DraftVersion.Elements!.Add(element);
                await context.Update(content);

                return await Task.FromResult(CreatedAtAction(nameof(GetContent), new { channelId, contentId }, element.ToContentElementDto()));
            }

            return await UsingContentContextAsync(channelId, contentId, contentContextFunctionAsync, Request.ETagHeader(), refreshETag: true, requiresAuthorisation: true);
        }

        [HttpGet("{contentId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContent(string channelId, string contentId)
        {
            IActionResult contentFunction(Content content)
            {
                var version = content.ToContentDto();

                if (version == null)
                {
                    return NotFound();
                }

                return Ok(version);
            }

            return await UsingContent(channelId, contentId, contentFunction);
        }

        [HttpGet("{contentId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentEdit(string channelId, string contentId)
        {
            IActionResult contentFunction(Content content)
            {
                return Ok(content.ToContentEditDto());
            }

            return await UsingContent(channelId, contentId, contentFunction, requiresAuthorisation: true);
        }

        [HttpGet("{contentId}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentPreview(string channelId, string contentId)
        {
            IActionResult contentFunction(Content content)
            {
                return Ok(content.ToContentDto(usePreview: true));
            }

            return await UsingContent(channelId, contentId, contentFunction, requiresAuthorisation: true);
        }
    }
}
