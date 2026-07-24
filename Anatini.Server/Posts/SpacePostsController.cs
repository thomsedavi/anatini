using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Posts
{
    [ApiController]
    [Route("api/spaces/{spaceId}/posts")]
    public class SpacePostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPost(string spaceId, [FromForm] CreatePost createPost) => await UsingSpaceAsync(spaceId, async (space) =>
        {
            var eventData = new EventData(HttpContext);

            var postId = Guid.CreateVersion7();

            Context.AddPost(postId, createPost.Name, createPost.Handle, space.Id);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { spaceId = space.Id, postId = createPost.Handle }, new { postId, DefaultHandle = createPost.Handle, createPost.Name });
        }, new ContextSettings { AccessRequired = true });

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
        public async Task<IActionResult> PatchPost(string spaceId, string postId, [FromForm] UpdatePost updatePost) => await UsingSpacePostAsync(spaceId, postId, async (post) =>
        {
            return NoContent();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{postId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string spaceId, string postId) => await UsingSpacePostAsync(spaceId, postId, async (post) =>
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
        public async Task<IActionResult> GetPostEdit(string spaceId, string postId) => await UsingSpacePostAsync(spaceId, postId, async (post) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{postId}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostPreview(string spaceId, string postId) => await UsingSpacePostAsync(spaceId, postId, async (post) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });
    }
}
