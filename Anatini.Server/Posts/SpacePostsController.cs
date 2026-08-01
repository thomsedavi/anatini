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
    [Route("api/spaces/{spaceHandle}/posts")]
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
        public async Task<IActionResult> PostPost(string spaceHandle, [FromForm] CreatePost createPost) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            var eventData = new EventData(HttpContext);

            var postId = Guid.CreateVersion7();

            Context.AddPost(postId, createPost.Name, createPost.Handle, space.Id);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { spaceId = space.Id, postId = createPost.Handle }, new { postId, DefaultHandle = createPost.Handle, createPost.Name });
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpPatch("{postHandle}")]
        [ETagRequired]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchPost(string spaceHandle, string postHandle, [FromForm] UpdatePost updatePost) => await UsingSpacePostAsync(spaceHandle, postHandle, async (post) =>
        {
            return NoContent();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{postHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPost(string spaceHandle, string postHandle) => await UsingSpacePostAsync(spaceHandle, postHandle, async (post) =>
        {
            return Ok();
        });

        [HttpGet("{postHandle}/edit")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostEdit(string spaceHandle, string postHandle) => await UsingSpacePostAsync(spaceHandle, postHandle, async (post) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });

        [HttpGet("{postHandle}/preview")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostPreview(string spaceHandle, string postHandle) => await UsingSpacePostAsync(spaceHandle, postHandle, async (post) =>
        {
            return Ok();
        }, new ContextSettings { AccessRequired = true });
    }
}
