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

        // add ETag headers
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/ETag
        [Authorize]
        [HttpPost("{contentId}/elements")]
        [ETagRequired]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostElement(string channelId, string contentId, [FromForm] NewElement newElement)
        {
            async Task<IActionResult> contentContextFunctionAsync(Content content, AnatiniContext context)
            {
                //var draftElement = new ContentOwnedElement
                //{
                //    ContentOwnedVersionContentChannelId = content.ChannelId,
                //    ContentOwnedVersionContentId = content.Id,
                //    Index = content.DraftVersion.Elements?.Count + 5 ?? 0,
                //    Tag = "h1"
                //};

                //content.DraftVersion.Elements?.Add(draftElement);

                //await context.Update(content);

                //var something = content.Aliases.First("slug");

                //await context.Update(content);

                //return await Task.FromResult(Ok());
                //var elements = content.Elements ?? [];
                //
                //elements.Add(new ContentOwnedElement { Index = 1, ContentChannelId = content.ChannelId, contentId = content.Id, Tag = "h1", Content = "Hello World" });
                //
                //content.Elements = elements;
                //
                //await context.Update(content);

                return await Task.FromResult(CreatedAtAction(nameof(GetContent), new { channelId, contentId }, newElement)); // value should be the created element
            }

            return await UsingContentContextAsync(channelId, contentId, contentContextFunctionAsync, Request.ETagHeader(), requiresAuthorisation: true);
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
                return Ok(content.ToContentDto());
            }

            return await UsingContent(channelId, contentId, contentFunction);
        }

        [HttpGet("{contentId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentEdit(string channelId, string contentId)
        {
            IActionResult contentFunction(Content content)
            {
                Response.Headers.ETag = content.ETag;

                return Ok(content.ToContentEditDto());
            }

            return await UsingContent(channelId, contentId, contentFunction, requiresAuthorisation: true);
        }
    }
}
