using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Posts.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/channels/{channelId}/posts")]
    public class ChannelPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : AnatiniControllerBase(context, userManager)
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

            var postId = Guid.CreateVersion7();

            context.AddPost(postId, createPost.Name, createPost.Handle, channel.Id, eventData);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { channelId = channel.Id, postId = createPost.Handle }, new { postId, DefaultHandle = createPost.Handle, createPost.Name });
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
            return NoContent();
        }, Request.ETagHeader(), refreshETag: true, requiresAccess: true);

        [HttpGet("{postId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string channelId, string postId) => await UsingPost(channelId, postId, post =>
        {
            return Ok();
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
            return Ok();
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
            return Ok();
        }, requiresAccess: true);
    }
}
