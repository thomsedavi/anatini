using System.Net.Mime;
using Anatini.Server.Channels.Extensions;
using Anatini.Server.Posts.Extensions;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/channels/{channelId}/posts")]
    public class ChannelPostsController : AnatiniControllerBase
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPost(string channelId, [FromForm] CreatePost createPost) => await UsingChannelContextAsync(channelId, async (channel, context) =>
        {
            var eventData = new EventData(HttpContext);

            var postAlias = await context.AddPostAliasAsync(createPost.Id, channel.Id, createPost.Handle, createPost.Name, createPost.Protected);
            var post = await context.AddPostAsync(createPost.Id, createPost.Name, createPost.Handle, createPost.Protected, channel.Id, eventData);

            channel.AddDraftPost(post, eventData);
            await context.UpdateAsync(channel);

            return CreatedAtAction(nameof(GetPost), new { channelId = channel.DefaultHandle, postId = createPost.Handle }, new { createPost.Id, DefaultHandle = createPost.Handle, createPost.Name });
        }, requiresAccess: true);

        [Authorize]
        [HttpPatch("{postId}")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchPost(string channelId, string postId, [FromForm] UpdatePost updatePost) => await UsingPostContextAsync(channelId, postId, async (post, channel, context) =>
        {
            if (updatePost.DateNZ.HasValue)
            {
                post.DraftVersion.DateNZ = updatePost.DateNZ.Value;
            }

            if (!string.IsNullOrEmpty(updatePost.Name))
            {
                post.DraftVersion.Name = updatePost.Name;
            }

            if (!string.IsNullOrEmpty(updatePost.Article))
            {
                post.DraftVersion.Article = updatePost.Article;
            }

            if (updatePost.Status == "Published")
            {
                if (post.PublishedVersion == null)
                {
                    await context.AddAttributePost(AttributePostType.Date, post.DraftVersion.DateNZ.GetDate(), channel, post);
                    await context.AddAttributePost(AttributePostType.Week, post.DraftVersion.DateNZ.GetWeek(), channel, post);
                }
                else if (post.PublishedVersion != null && (post.PublishedVersion.DateNZ != post.DraftVersion.DateNZ || post.PublishedVersion.Name != post.DraftVersion.Name || post.PublishedVersion.CardImageId != post.DraftVersion.CardImageId))
                {
                    await context.RemoveAttributePost(AttributePostType.Date, post.PublishedVersion.DateNZ.GetDate(), channel, post);
                    await context.RemoveAttributePost(AttributePostType.Week, post.PublishedVersion.DateNZ.GetWeek(), channel, post);
                    await context.AddAttributePost(AttributePostType.Date, post.DraftVersion.DateNZ.GetDate(), channel, post);
                    await context.AddAttributePost(AttributePostType.Week, post.DraftVersion.DateNZ.GetWeek(), channel, post);
                }

                var publishedVersion = new PostOwnedVersion
                {
                    Name = post.DraftVersion.Name,
                    PostId = post.Id,
                    PostChannelId = post.ChannelId,
                    DateNZ = post.DraftVersion.DateNZ,
                    Article = post.DraftVersion.Article
                };

                post.Status = updatePost.Status;
                post.PublishedVersion = publishedVersion;
            }
            else if (updatePost.Status == "Draft")
            {
                if (post.PublishedVersion != null)
                {
                    await context.RemoveAttributePost(AttributePostType.Date, post.PublishedVersion.DateNZ.GetDate(), channel, post);
                    await context.RemoveAttributePost(AttributePostType.Week, post.PublishedVersion.DateNZ.GetWeek(), channel, post);
                }

                post.Status = updatePost.Status;
                post.PublishedVersion = null;
            }

            await context.UpdateAsync(post);

            return NoContent();
        }, Request.ETagHeader(), refreshETag: true, requiresAccess: true);

        [HttpGet("{postId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string channelId, string postId) => await UsingPost(channelId, postId, post =>
        {
            var versionDto = post.ToPostDto();

            if (versionDto == null)
            {
                return NotFound();
            }

            return Ok(versionDto);
        });

        [HttpGet("{postId}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostEdit(string channelId, string postId) => await UsingPost(channelId, postId, post =>
        {
            return Ok(post.ToPostEditDto());
        }, requiresAccess: true);

        [HttpGet("{postId}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostPreview(string channelId, string postId) => await UsingPost(channelId, postId, post =>
        {
            return Ok(post.ToPostDto(usePreview: true));
        }, requiresAccess: true);
    }
}
