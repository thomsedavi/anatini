using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Spaces
{
    [ApiController]
    [Route("api/users/{userHandle}/spaces")]
    public class UserSpacesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSpace(string userHandle, [FromForm] CreateSpace createSpace) => await UsingUserAsync(userHandle, async (user) =>
        {
            var space = Context.AddSpace(user.Id, createSpace.Name, createSpace.Visibility, NormalizeHandleOrNull(createSpace.Handle));

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(SpacesController.GetSpace), new { spaceHandle = space.Handle }, await space.ToSpaceDtoAsync(IsAuthenticated, BlobService));
        }, new ContextSettings { AccessRequired = true });

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpaces(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            var spacesQuery = Context.Spaces.AsQueryable();

            spacesQuery = spacesQuery.AsNoTracking();

            spacesQuery = spacesQuery.Where(space => space.UserRelationships.Any(userSpaceRelationship => userSpaceRelationship.SourceUserId == user.Id && userSpaceRelationship.Label == UserSpaceRelationshipLabel.IsOwned));

            var spaces = await spacesQuery.ToListAsync();

            if (spaces == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(spaces.Select(post => post.ToSpaceDtoAsync(IsAuthenticated, BlobService))));
        });
    }
}
